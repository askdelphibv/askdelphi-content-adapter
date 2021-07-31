using AskDelphi.Adapter.AzureNuggets.DTO;
using Azure.Storage.Files.Shares;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets
{
    public class AzureFilesProvider : IAzureFilesProvider
    {
        private readonly IConfiguration configuration;

        private string cachedSasUIR;
        private ShareClient cachedShareClient;

        public AzureFilesProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<(bool success, IEnumerable<FolderDescriptor> folders, IEnumerable<ResourceDescriptor> resources)> FindChildren(IOperationContext operationContext, string folderId)
        {
            List<FolderDescriptor> resultFolders = new List<FolderDescriptor>();
            List<ResourceDescriptor> resultResouces = new List<ResourceDescriptor>();

            ShareClient client = GetClient();
            ShareDirectoryClient directoryClient = string.IsNullOrEmpty(folderId?.Trim('/')) ? client.GetRootDirectoryClient() : client.GetDirectoryClient(folderId);

            Azure.AsyncPageable<Azure.Storage.Files.Shares.Models.ShareFileItem> azureFiles = directoryClient.GetFilesAndDirectoriesAsync();
            await foreach (var azureFileEntry in azureFiles)
            {
                if (azureFileEntry.IsDirectory)
                {
                    ShareDirectoryClient subDirectoryClient = directoryClient.GetSubdirectoryClient(azureFileEntry.Name);
                    resultFolders.Add(ToFolderDescriptor(folderId ?? "/", azureFileEntry, subDirectoryClient));
                }
                else
                {
                    ShareFileClient fileClient = directoryClient.GetFileClient(azureFileEntry.Name);
                    resultResouces.Add(ToFileDescriptor(folderId ?? "/", azureFileEntry, fileClient));
                }
            }

            return (success: true, folders: resultFolders.OrderBy(x => x.Name).ThenBy(x => x.FolderId), resources: resultResouces.OrderBy(x => x.Filename).ThenBy(x => x.ResourceId));
        }

        public async Task<ResourceDescriptor> GetResourceDescriptor(IOperationContext operationContext, string resourceId)
        {
            (string folderId, string filename) = SplitPath(resourceId);
            if (string.IsNullOrWhiteSpace(folderId)) folderId = "/";

            ShareDirectoryClient directoryClient = GetClient().GetDirectoryClient(folderId);
            ShareFileClient fileClient = directoryClient.GetFileClient(filename);
            Azure.Response<Azure.Storage.Files.Shares.Models.ShareFileProperties> properties = fileClient.GetProperties();

            var result = new ResourceDescriptor
            {
                ResourceId = resourceId,
                ContentLength = (int)properties.Value.ContentLength,
                Filename = fileClient.Name,
                Status = "Final",
                LastModified = properties.Value.LastModified.UtcDateTime.ToString("o"),
                MimeType = properties.Value.ContentType
            };
            return await Task.FromResult<ResourceDescriptor>(result);
        }

        public async Task<FolderDescriptor> GetFolderDescriptor(IOperationContext operationContext, string subFolderId)
        {
            (string folderId, string filename) = SplitPath(subFolderId);
            if (string.IsNullOrWhiteSpace(folderId)) folderId = "/";

            ShareDirectoryClient directoryClient = GetClient().GetDirectoryClient(folderId);
            var subDirectoryClient = directoryClient.GetSubdirectoryClient(filename);
            var result = ToFolderDescriptor(folderId, subDirectoryClient);
            return await Task.FromResult(result);
        }

        public async Task<ResourceMetadata> GetResourceMetadata(IOperationContext operationContext, string resourceId)
        {
            var meta = new ResourceMetadata
            {
                Content = "",
                Description = "",
                Tags = new TaxonomyValues[] { }
            };
            return await Task.FromResult<ResourceMetadata>(meta);
        }

        public async Task<Stream> OpenStream(IOperationContext operationContext, string resourceId)
        {
            (string folderId, string filename) = SplitPath(resourceId);
            if (string.IsNullOrWhiteSpace(folderId)) folderId = "/";

            ShareDirectoryClient directoryClient = GetClient().GetDirectoryClient(folderId);
            ShareFileClient fileClient = directoryClient.GetFileClient(filename);

            return await fileClient.OpenReadAsync(new Azure.Storage.Files.Shares.Models.ShareFileOpenReadOptions(true));
        }

        public async Task<(bool success, IEnumerable<ResourceDescriptor> resources, int totalCount)> Search(IOperationContext operationContext, string folderId, string requestQuery, int requestPage, int requestSize)
        {
            // TODO: Ideally there is an indexing service present that helps us search for matching resouces, we'll not just do a paged BFS
            if (string.IsNullOrEmpty(requestQuery) || requestQuery.Length < 1)
            {
                return (true, new ResourceDescriptor[] { }, 0);
            }

            BFSContext context = new BFSContext
            {
                Query = requestQuery,
                SkippedResults = requestPage * requestSize,
                RequestedResults = requestSize,
                Result = new List<ResourceDescriptor>()
            };

            var directoryClient = string.IsNullOrEmpty(folderId?.Trim('/'))
                ? GetClient().GetRootDirectoryClient()
                : GetClient().GetDirectoryClient(folderId);
            bool success = await BFS(operationContext, directoryClient, "/", context);
            return (success, context.Result, -1); // Total count is not supported until we use an actual indexing service, so we return -1 which is according to specification in case it's not supported.
        }


        // TODO: Should refactor for testability, should be a machine, not a method.

        private async Task<bool> BFS(IOperationContext operationContext, ShareDirectoryClient directoryClient, string folderId, BFSContext context)
        {
            List<Azure.Storage.Files.Shares.Models.ShareFileItem> folders = new List<Azure.Storage.Files.Shares.Models.ShareFileItem>();
            Azure.AsyncPageable<Azure.Storage.Files.Shares.Models.ShareFileItem> azureFiles = directoryClient.GetFilesAndDirectoriesAsync();
            await foreach (Azure.Storage.Files.Shares.Models.ShareFileItem azureFileEntry in azureFiles)
            {
                if (context.RequestedResults == 0) return true;

                if (azureFileEntry.IsDirectory)
                {
                    folders.Add(azureFileEntry);
                }
                else if (IsFileEntrySearchMatchForQuery(context, azureFileEntry))
                {
                    if (context.SkippedResults > 0) // Skipped, don't spend time on it...
                    {
                        context.SkippedResults--;
                    }
                    else
                    {
                        ShareFileClient fileClient = directoryClient.GetFileClient(azureFileEntry.Name);
                        context.Result.Add(ToFileDescriptor(folderId ?? "/", azureFileEntry, fileClient));
                        context.RequestedResults--;
                    }

                }
            }

            foreach (var folder in folders)
            {
                if (context.RequestedResults == 0) return true;

                bool success = await BFS(operationContext, directoryClient.GetSubdirectoryClient(folder.Name), $"{folderId.TrimEnd('/')}/{folder.Name}", context);
                if (!success)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsFileEntrySearchMatchForQuery(BFSContext context, Azure.Storage.Files.Shares.Models.ShareFileItem azureFileEntry)
        {
            return azureFileEntry.Name.IndexOf(context.Query, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }

        private ShareClient GetClient()
        {
            string configuredSasUri = configuration.GetValue<string>("AzureFiles:SASUri");
            if (configuredSasUri != cachedSasUIR)
            {
                cachedSasUIR = configuredSasUri;
                cachedShareClient = new ShareClient(new Uri(configuredSasUri));
            }
            return cachedShareClient;
        }

        private ResourceDescriptor ToFileDescriptor(string folderId, Azure.Storage.Files.Shares.Models.ShareFileItem azureFileEntry, ShareFileClient fileClient)
        {
            Azure.Response<Azure.Storage.Files.Shares.Models.ShareFileProperties> properties = fileClient.GetProperties();

            return new ResourceDescriptor
            {
                ResourceId = $"{folderId.TrimEnd('/')}/{azureFileEntry.Name}",
                ContentLength = (int)azureFileEntry.FileSize,
                Filename = azureFileEntry.Name,
                Status = "Final",
                LastModified = properties.Value.LastModified.UtcDateTime.ToString("o"),
                MimeType = properties.Value.ContentType
            };
        }

        private FolderDescriptor ToFolderDescriptor(string folderId, Azure.Storage.Files.Shares.Models.ShareFileItem azureFileEntry, ShareDirectoryClient subDirectoryClient)
        {
            Azure.Response<Azure.Storage.Files.Shares.Models.ShareDirectoryProperties> properties = subDirectoryClient.GetProperties();
            return new FolderDescriptor
            {
                FolderId = $"{folderId.TrimEnd('/')}/{azureFileEntry.Name}",
                Name = azureFileEntry.Name,
                LastModified = properties.Value.LastModified.UtcDateTime.ToString("o")
            };
        }

        private FolderDescriptor ToFolderDescriptor(string folderId, ShareDirectoryClient subDirectoryClient)
        {
            Azure.Response<Azure.Storage.Files.Shares.Models.ShareDirectoryProperties> properties = subDirectoryClient.GetProperties();
            return new FolderDescriptor
            {
                FolderId = $"{folderId.TrimEnd('/')}/{subDirectoryClient.Name}",
                Name = subDirectoryClient.Name,
                LastModified = properties.Value.LastModified.UtcDateTime.ToString("o")
            };
        }

        public (string directory, string file) SplitPath(string fullPath)
        {
            var match = Regex.Match(fullPath, "^(.*)[/]([^/]*)$");
            if (match.Success)
            {
                return (match.Groups[1].Value?.TrimEnd('/'), match.Groups[2].Value.Trim('/'));
            }
            return (string.Empty, fullPath);
        }

        private class BFSContext
        {
            internal string Query;
            internal int SkippedResults; // until this reaches zero, add noting
            internal int RequestedResults; // if this reaches 0 we're done
            internal List<ResourceDescriptor> Result;
        }
    }
}
