using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class AuthLogoutSuccessResponse : APISuccessResponseBase
    {
        public AuthLogoutSuccessResponse(IOperationContext operationContext) : base(Constants.APIVersion1, operationContext) { }
    }
}
