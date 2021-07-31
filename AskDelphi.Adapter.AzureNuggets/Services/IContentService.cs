using AskDelphi.Adapter.AzureNuggets.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services
{
    /// <summary>
    /// Implements the content service powering the API's content endpoints.
    /// </summary>
    public interface IContentService
    {
        /// <summary>
        /// Returns the contents of a single virtual folder of topics in the customer CMS system. 
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="response"></param>
        /// <param name="folderId"></param>
        /// <returns></returns>
        Task<bool> ListFolders(IOperationContext operationContext, ContentFoldersSuccessResponse response, string folderId);

        /// <summary>
        /// Returns the topic-contents of a single virtual folder of topics in the customer CMS system. 
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="response"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<bool> Search(IOperationContext operationContext, ContentSearchSuccessResponse response, ContentSearchRequest request);

        /// <summary>
        /// Returns metadata for a single topic. 
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="response"></param>
        /// <param name="topicId"></param>
        /// <returns></returns>
        Task<bool> GetMetadata(IOperationContext operationContext, ContentMetadataSuccessResponse response, string topicId);

        /// <summary>
        /// Returns all topic contents required for rendering for a single topic. This will have to convert the customer CMS-data into a format that’s usable in AskDelphi, as defined in the content specification.
        /// </summary>
        /// <param name="operationContext"></param>
        /// <param name="response"></param>
        /// <param name="topicId"></param>
        /// <returns></returns>
        Task<bool> GetContent(IOperationContext operationContext, ContentContentSuccessResponse response, string topicId);
    }
}
