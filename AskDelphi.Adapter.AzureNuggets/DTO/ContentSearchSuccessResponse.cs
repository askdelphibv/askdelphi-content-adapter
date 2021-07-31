using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ContentSearchSuccessResponse : APISuccessResponseBase
    {
        public ContentSearchSuccessResponse(IOperationContext operationContext) : base(Constants.APIVersion1, operationContext) { }

        public int TotalCount { get; set; }
        
        public string ContinuationToken { get; set; }

        public TopicDescriptor[] Topics { get; set; }
    }
}
