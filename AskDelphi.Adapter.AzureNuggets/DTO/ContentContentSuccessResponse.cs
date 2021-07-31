using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class ContentContentSuccessResponse : APISuccessResponseBase
    {
        public ContentContentSuccessResponse(IOperationContext operationContext) : base(Constants.APIVersion1, operationContext) { }

        public TopicContent[] Contents { get; set; }

    }
}
