using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ResourcesSearchSuccessResponse : APISuccessResponseBase
    {
        public ResourcesSearchSuccessResponse(IOperationContext operationContext) : base(Constants.APIVersion1, operationContext) { }

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public string ContinuationToken { get; set; }

        public ResourceDescriptor[] Resouces { get; set; }
    }
}
