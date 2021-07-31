using AskDelphi.Adapter.AzureNuggets.DTO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets
{
    public class ResourcesService : IResourcesService
    {
        private readonly ILogger<ResourcesService> logger;
        private readonly IAzureFilesProvider azureFilesProvider;

        public ResourcesService(ILogger<ResourcesService> logger, IAzureFilesProvider azureFilesProvider)
        {
            this.logger = logger;
            this.azureFilesProvider = azureFilesProvider;
        }

        public async Task<bool> GetMetadata(IOperationContext operationContext, ResourcesMetadataSuccessResponse response, string resourceId)
        {
            ResourceMetadata meta = await azureFilesProvider.GetResourceMetadata(operationContext, resourceId);
            response.Meta = meta;
            return meta != null;
        }

        public async Task<ResourceDescriptor> GetResouceDescriptor(IOperationContext operationContext, string resourceId)
        {
            try
            {
                return await azureFilesProvider.GetResourceDescriptor(operationContext, resourceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{operationContext.OperationId} failed");
                return null;
            }
        }

        public async Task<Stream> GetResourceStream(IOperationContext operationContext, string resourceId)
        {
            return await azureFilesProvider.OpenStream(operationContext, resourceId);
        }

        public async Task<bool> List(IOperationContext operationContext, ResourcesListSuccessResponse response, string folderId)
        {
            try
            {
                (bool success, IEnumerable<FolderDescriptor> folders, IEnumerable<ResourceDescriptor> resources) = await azureFilesProvider.FindChildren(operationContext, folderId);
                if (success)
                {
                    response.Resources = resources.ToArray();
                    response.Folders = folders.ToArray();
                }
                return success;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{operationContext.OperationId} failed");
                return false;
            }
        }

        public async Task<bool> Search(IOperationContext operationContext, ResourcesSearchSuccessResponse response, string query, int page, int size, string continuationToken)
        {
            int requestPage = page;
            int requestSize = size;
            string requestQuery = query;

            Match match = Regex.Match(continuationToken ?? string.Empty, @"^P([0-9]{1,}):S([0-9]{1,}):Q(.*):$");
            if (match.Success)
            {
                int.TryParse(match.Groups[1].Value, out requestPage);
                requestPage++;

                int.TryParse(match.Groups[2].Value, out requestSize);
                requestQuery = match.Groups[3].Value;
            }

            (bool success, IEnumerable<ResourceDescriptor> resources, int totalCount) = await azureFilesProvider.Search(operationContext, string.Empty, requestQuery, requestPage, requestSize);
            if (success)
            {
                response.Resouces = resources.ToArray();
                response.ContinuationToken = $"P{requestPage}:S{requestSize}:Q{requestQuery}:";
                response.Page = requestPage;
                response.TotalCount = totalCount;
            }

            return success;
        }
    }
}
