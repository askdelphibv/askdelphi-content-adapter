using AskDelphi.Adapter.AzureNuggets.DTO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets
{
    public interface IAzureFilesProvider
    {
        Task<(bool success, IEnumerable<FolderDescriptor> folders, IEnumerable<ResourceDescriptor> resources)> FindChildren(IOperationContext operationContext, string folderId);
        Task<Stream> OpenStream(IOperationContext operationContext, string resourceId);
        Task<ResourceDescriptor> GetResourceDescriptor(IOperationContext operationContext, string resourceId);
        Task<ResourceMetadata> GetResourceMetadata(IOperationContext operationContext, string resourceId);
        Task<(bool success, IEnumerable<ResourceDescriptor> resources, int totalCount)> Search(IOperationContext operationContext, string folderId, string requestQuery, int requestPage, int requestSize);
        (string directory, string file) SplitPath(string fullPath);
        Task<FolderDescriptor> GetFolderDescriptor(IOperationContext operationContext, string folderId);
    }
}