using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ResourcesListSuccessResponse : APISuccessResponseBase
    {
        public ResourcesListSuccessResponse(IOperationContext operationContext) : base(Constants.APIVersion1, operationContext) { }

        public FolderDescriptor[] Folders { get; set; }

        public ResourceDescriptor[] Resources { get; set; }
    }
}
