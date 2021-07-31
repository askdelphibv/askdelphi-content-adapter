using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class APIErrorResponse : IAPIResponse
    {
        public bool Success => Constants.Fail;
        public string Version => Constants.APIVersion1;

        public string Code { get; set; }

        public string Message { get; set; }

        public string Id { get; set; }

        public APIErrorResponse(IOperationContext operationContext, string code, string message)
        {
            Code = code;
            Message = message;
            Id = operationContext.OperationId;
        }
    }
}
