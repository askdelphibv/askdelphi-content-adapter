using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public abstract class APISuccessResponseBase : IAPIResponse
    {
        private readonly string version;
        protected readonly IOperationContext operationContext;

        public bool Success => Constants.Success;
        public string Version => version;
        public string Id => operationContext.OperationId;

        public APISuccessResponseBase(string version, IOperationContext operationContext)
        {
            this.version = version;
            this.operationContext = operationContext;
        }
    }
}
