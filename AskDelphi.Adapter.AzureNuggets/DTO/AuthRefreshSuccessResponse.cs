using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class AuthRefreshSuccessResponse : APISuccessResponseBase
    {
        public AuthRefreshSuccessResponse(IOperationContext operationContext) : base(Constants.APIVersion1, operationContext) { }

        public string Token { get; set; }

    }
}
