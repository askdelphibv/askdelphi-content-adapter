using AskDelphi.Adapter.AzureNuggets.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets
{
    public interface INuggetContentProvider
    {
        Task<(bool success, IEnumerable<FolderDescriptor> folders)> FindContentFolders(IOperationContext operationContext, string parentId);
        Task<(bool success, IEnumerable<TopicDescriptor> topics, int totalCount, string continuationToken)> Search(IOperationContext operationContext, ContentSearchRequest request);
        Task<(bool success, TopicMetadata meta)> GetTopicMetadata(IOperationContext operationContext, string topicId);
        Task<(bool success, IEnumerable<TopicContent> contents)> GetTopicsForNugget(IOperationContext operationContext, string topicId);
        Task SynchronizeCache(IOperationContext operationContext);
    }
}