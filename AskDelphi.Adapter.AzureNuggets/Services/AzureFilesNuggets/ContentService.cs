using AskDelphi.Adapter.AzureNuggets.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets
{
    public class ContentService : IContentService
    {
        private readonly ILogger<ContentService> logger;
        private readonly INuggetContentProvider nuggetContentProvider;

        public ContentService(ILogger<ContentService> logger, INuggetContentProvider nuggetContentProvider)
        {
            this.logger = logger;
            this.nuggetContentProvider = nuggetContentProvider;
        }

        public async Task<bool> ListFolders(IOperationContext operationContext, ContentFoldersSuccessResponse response, string folderId)
        {
            try
            {
                (bool success, IEnumerable<FolderDescriptor> folders) = await nuggetContentProvider.FindContentFolders(operationContext, folderId);
                if (success)
                {
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

        public async Task<bool> GetMetadata(IOperationContext operationContext, ContentMetadataSuccessResponse response, string topicId)
        {
            try
            {
                (bool success, TopicMetadata meta) = await nuggetContentProvider.GetTopicMetadata(operationContext, topicId);
                if (success)
                {
                    response.Meta = meta;
                }
                return success;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{operationContext.OperationId} failed");
                return false;
            }
        }

        public async Task<bool> Search(IOperationContext operationContext, ContentSearchSuccessResponse response, ContentSearchRequest request)
        {
            try
            {
                (bool success, IEnumerable<TopicDescriptor> topics, int totalCount, string continuationToken) = await nuggetContentProvider.Search(operationContext, request);
                if (success)
                {
                    response.ContinuationToken = continuationToken;
                    response.Topics = topics.ToArray();
                    response.TotalCount = totalCount;
                }
                return success;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{operationContext.OperationId} failed");
                return false;
            }
        }

        public async Task<bool> GetContent(IOperationContext operationContext, ContentContentSuccessResponse response, string topicId)
        {
            try
            {
                (bool success, IEnumerable<TopicContent> contents) = await nuggetContentProvider.GetTopicsForNugget(operationContext, topicId);
                if (success)
                {
                    response.Contents = contents?.ToArray();
                }
                return success;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"{operationContext.OperationId} failed");
                return false;
            }
        }
    }
}
