using AskDelphi.Adapter.AzureNuggets.DTO;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services
{
    /// <summary>
    /// Implements support for the resource endpoints.
    /// </summary>
    public interface IResourcesService
    {
        /// <summary>
        /// Returns the contents of a single virtual folder of resources in the customer CMS system. 
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="response"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        Task<bool> List(IOperationContext operationContext, ResourcesListSuccessResponse response, string folderId);

        /// <summary>
        /// Searches the customer CMS for resources that best match the specified query. It’s up to the implementing system to decide how this query is interpreted and which fields and metadata are involved in searching.
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="response"></param>
        /// <param name="query"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="continuationToken"></param>
        /// <returns></returns>
        Task<bool> Search(IOperationContext operationContext, ResourcesSearchSuccessResponse response, string query, int page, int size, string continuationToken);

        /// <summary>
        /// Returns metadata for a single resource. 
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="response"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        Task<bool> GetMetadata(IOperationContext operationContext, ResourcesMetadataSuccessResponse response, string resourceId);
        
        /// <summary>
        /// Returns a resource descriptor with all details for a single resource. Not part of the API but required for returning content.
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        Task<ResourceDescriptor> GetResouceDescriptor(IOperationContext operationContext, string resourceId);

        /// <summary>
        /// This method is the primary method to retrieve the binary contents of a resource. Returns a stream for the resource, the stream should be random-access.
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        Task<Stream> GetResourceStream(IOperationContext operationContext, string resourceId);
    }
}
