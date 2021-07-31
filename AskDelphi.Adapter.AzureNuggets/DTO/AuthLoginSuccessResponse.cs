using AskDelphi.Adapter.AzureNuggets.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.DTO
{
    public class AuthLoginSuccessResponse : APISuccessResponseBase
    {
        public AuthLoginSuccessResponse(IOperationContext operationContext) : base(Constants.APIVersion1, operationContext) { }

        /// <summary>
        /// A valid JWT token created just for this application.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Not supporting refresh tokens
        /// </summary>
        public string Refresh => null;
    }
}
