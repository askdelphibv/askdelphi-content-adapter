using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ContentFoldersSuccessResponse : APISuccessResponseBase
    {
        public ContentFoldersSuccessResponse(IOperationContext operationContext) : base(Constants.APIVersion1, operationContext) { }

        public FolderDescriptor[] Folders { get; set; }
    }
}
