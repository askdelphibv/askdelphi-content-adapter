using AskDelphi.Adapter.AzureNuggets.DTO;
using AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets.Imola;
using AskDelphi.Adapter.AzureNuggets.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets
{
    public class NuggetContentProvider : INuggetContentProvider
    {
        private readonly ILogger<NuggetContentProvider> logger;
        private readonly IAzureFilesProvider azureFilesProvider;
        private readonly IMemoryCacheService memoryCacheService;
        private readonly IConfiguration configuration;
        private readonly Semaphore indexLock = new Semaphore(1, 1);

        public NuggetContentProvider(ILogger<NuggetContentProvider> logger, IAzureFilesProvider azureFilesProvider, IMemoryCacheService memoryCacheService, IConfiguration configuration)
        {
            this.logger = logger;
            this.azureFilesProvider = azureFilesProvider;
            this.memoryCacheService = memoryCacheService;
            this.configuration = configuration;
        }

        public async Task<(bool success, IEnumerable<FolderDescriptor> folders)> FindContentFolders(IOperationContext operationContext, string parentId)
        {
            string prefix = string.IsNullOrWhiteSpace(parentId) ? "/" : (parentId.TrimEnd('/') + '/');
            var result = new List<FolderDescriptor>();
            List<NuggetData> index = await GetIndexAsync(operationContext);
            var folderIds = index
                .Where(x => x.Id.StartsWith(prefix))
                .Select(x =>
                {
                    // remove nugget.json
                    var (folder, _) = azureFilesProvider.SplitPath(x.Id);
                    return folder;
                })
                .Select(x =>
                {
                    // remove the containing folder (this actually is the nugget)
                    var (folder, _) = azureFilesProvider.SplitPath(x);
                    if (string.IsNullOrWhiteSpace(folder))
                    {
                        folder = "/";
                    }
                    return folder;
                })
                .Distinct()
                .ToArray();

            foreach (var folderId in folderIds)
            {
                result.Add(await azureFilesProvider.GetFolderDescriptor(operationContext, folderId));
            }

            return (true, result);
        }

        public async Task<(bool success, IEnumerable<TopicDescriptor> topics, int totalCount, string continuationToken)> Search(IOperationContext operationContext, ContentSearchRequest request)
        {
            List<NuggetData> index = await GetIndexAsync(operationContext);

            if (!string.IsNullOrWhiteSpace(request.ContinuationToken))
            {
                request = Newtonsoft.Json.JsonConvert.DeserializeObject<ContentSearchRequest>(request.ContinuationToken);
            }

            string nuggetTopicType = configuration.GetValue<string>("Configuration:NuggetTopicType");
            var matchingNuggets = index.Where(nuggetData => IsMatchForSearchRequest(operationContext, request, nuggetData, nuggetTopicType));
            int totalCount = matchingNuggets.Count();

            IEnumerable<TopicDescriptor> topics = matchingNuggets
                .OrderBy(x => x.Nugget.Application)
                .ThenBy(x => x.Nugget.Title)
                .ThenBy(x => x.Nugget.Version)
                .ThenBy(x => x.Nugget.Uuid)
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .Select(nuggetData =>
            {
                return new TopicDescriptor
                {
                    Namespace = Namespaces.NamespaceNugget,
                    Title = nuggetData.Nugget.Title,
                    Status = "Final",
                    TopicId = nuggetData.Id,
                    Type = nuggetTopicType,
                    Version = "1.0"
                };
            }).ToArray();

            request.Page++;
            request.ContinuationToken = null;
            string continuationToken = Newtonsoft.Json.JsonConvert.SerializeObject(request);

            return (true, topics, totalCount, continuationToken);
        }

        public async Task<(bool success, TopicMetadata meta)> GetTopicMetadata(IOperationContext operationContext, string topicId)
        {
            List<NuggetData> index = await GetIndexAsync(operationContext);

            NuggetData nugget = index.Single(x => x.Id == topicId);
            string keywords = string.Join(" ", (nugget.Nugget.Keywords ?? new List<Nugget.NuggetTextAndImage>()).Select(x => CleanupHtmlTags(x.Text)));
            string steps = string.Join(" ", (nugget.Nugget.Steps ?? new List<Nugget.NuggetStep>()).Select(x => $"{CleanupHtmlTags(x.Text)} {CleanupHtmlTags(x.Clickinstruction)} {CleanupHtmlTags(x.Alt)}"));
            TopicMetadata result = new TopicMetadata
            {
                Description = CleanupHtmlTags(nugget.Nugget.Description),
                IndexContents = $"{nugget.Nugget.Application} {CleanupHtmlTags(nugget.Nugget.Title)} {CleanupHtmlTags(nugget.Nugget.Description)} {keywords} {steps}",
                Tags = new TaxonomyValues[] { } // TODO: Support tags
            };
            return (true, result);
        }

        public async Task<(bool success, IEnumerable<TopicContent> contents)> GetTopicsForNugget(IOperationContext operationContext, string topicId)
        {
            List<NuggetData> index = await GetIndexAsync(operationContext);
            var nuggetData = index.Single(x => x.Id == topicId);

            var (resourceFolder, _) = azureFilesProvider.SplitPath(topicId);

            var factory = new NuggetTopicFactory(operationContext, configuration, nuggetData.Nugget, topicId, resourceFolder);
            var contents = await factory.Build();

            return (true, contents);
        }

        private string CleanupHtmlTags(string str)
        {
            string result = str;
            result = Regex.Replace(result, "&#([0-9]{1,});", (x) => $"{(char)int.Parse(x.Groups[1].Value)}");
            result = Regex.Replace(result, "<b>|</b>|<i>|</i>", "", RegexOptions.IgnoreCase);
            result = System.Web.HttpUtility.HtmlDecode(result);
            return result;
        }

        private bool IsMatchForSearchRequest(IOperationContext operationContext, ContentSearchRequest request, NuggetData nuggetData, string nuggetTopicType)
        {
            if (!string.IsNullOrWhiteSpace(request.FolderId) && !(nuggetData.Id.StartsWith(request.FolderId)))
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(request.Query) && !(
                    nuggetData.Id.IndexOf(request.Query, StringComparison.InvariantCultureIgnoreCase) > 0
                    || (nuggetData.Nugget.Title ?? "").IndexOf(request.Query, StringComparison.InvariantCultureIgnoreCase) > 0
                    || (nuggetData.Nugget.Description ?? "").IndexOf(request.Query, StringComparison.InvariantCultureIgnoreCase) > 0
                    ))
            {
                return false;
            }
            if (null != request.TopicTypes && !request.TopicTypes.Contains(nuggetTopicType))
            {
                return false;
            }

            // TODO
            // request.Tags, but this requires topic-level sync

            return true;
        }

        private async Task<List<NuggetData>> GetIndexAsync(IOperationContext operationContext)
        {
            NuggetIndexCacheKey cacheKey = new NuggetIndexCacheKey();

            List<NuggetData> index = await memoryCacheService.GetAsync<List<NuggetData>>(cacheKey);
            if (index == default(List<NuggetData>))
            {
                if (!indexLock.WaitOne(TimeSpan.FromMinutes(2)))
                {
                    throw new TimeoutException($"Could not obtain lock!");
                }

                try
                {
                    logger.LogInformation($"{operationContext.OperationId} Re-building nugget index, this could take a while");
                    index = await memoryCacheService.GetAsync<List<NuggetData>>(cacheKey);
                    if (index == default(List<NuggetData>))
                    {
                        index = await UpdateCache(operationContext, cacheKey);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{operationContext.OperationId} Error re-building the index.");
                    throw;
                }
                finally
                {
                    logger.LogInformation($"{operationContext.OperationId} After re-building the index, release the lock.");
                    indexLock.Release();
                }
            }
            return index;
        }

        private async Task<List<NuggetData>> UpdateCache(IOperationContext operationContext, NuggetIndexCacheKey cacheKey)
        {
            List<NuggetData> index = await BuildIndex(operationContext);
            await memoryCacheService.SetWithAbsoluteExpirationAsync(cacheKey, index, DateTimeOffset.UtcNow + TimeSpan.FromDays(2));
            return index;
        }

        public async Task SynchronizeCache(IOperationContext operationContext)
        {
            if (indexLock.WaitOne(TimeSpan.FromMinutes(5)))
            {
                try
                {
                    logger.LogInformation($"{operationContext.OperationId} Synchronizing nugget index, this could take a while");
                    NuggetIndexCacheKey cacheKey = new NuggetIndexCacheKey();
                    await UpdateCache(operationContext, cacheKey);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{operationContext.OperationId} Failed synchronizing the index.");
                }
                finally
                {
                    logger.LogInformation($"{operationContext.OperationId} After synchronizing the index, releasing the lock.");
                    indexLock.Release();

                }
            }
            else
            {
                logger.LogError($"Could not obtain the lock while trying to start a synchronization action on the nugget store.");
            }
        }

        private async Task<List<NuggetData>> BuildIndex(IOperationContext operationContext)
        {
            List<NuggetData> nuggets = new List<NuggetData>();
            var (_, resources, _) = await azureFilesProvider.Search(operationContext, string.Empty, "nugget.json", 0, Int32.MaxValue);
            foreach (var resource in resources)
            {
                try
                {
                    using (var resourceStream = await azureFilesProvider.OpenStream(operationContext, resource.ResourceId))
                    {
                        using (var reader = new System.IO.StreamReader(resourceStream))
                        {
                            string nuggetText = reader.ReadToEnd();
                            Nugget nugget = Newtonsoft.Json.JsonConvert.DeserializeObject<Nugget>(nuggetText);
                            nuggets.Add(new NuggetData
                            {
                                Id = resource.ResourceId,
                                Nugget = nugget
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"{operationContext.OperationId} Couldn't process resource {resource.ResourceId}.");
                }
            }
            return nuggets;
        }

        private class NuggetIndexCacheKey : CacheKey
        {
            public override string Region => nameof(NuggetIndexCacheKey);
        }

        private class NuggetData
        {
            public string Id { get; set; }
            public Nugget Nugget { get; set; }
        }
    }
}
