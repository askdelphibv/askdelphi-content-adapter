using AskDelphi.Adapter.AzureNuggets.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services
{
    public class OperationContextFactory : IOperationContextFactory
    {
        public IOperationContext CreateBackgroundOperationContext()
        {
            return new OperationContext();
        }

        public IOperationContext CreateOperationContext(HttpContext context)
        {
            var result = new OperationContext();
            result.InitializeClaims(context.User.Identity as ClaimsIdentity);
            return result;
        }

        private class OperationContext : IOperationContext
        {
            private string contextId = $"{Guid.NewGuid()}";
            private string authToken;
            private string askDelphiSystemID;
            private IEnumerable<ClaimTuple> claims = new ClaimTuple[] { };

            public string OperationId => contextId;

            public OperationContext()
            {
            }

            public void Dispose() { }

            public string GetAuthToken() => authToken;

            public string GetAskDelphiSystemID() => askDelphiSystemID;

            public void InitializeFromRequest(HttpRequest request)
            {
                this.authToken = request.Headers.FirstOrDefault(x => string.Equals(x.Key, "Authorization", StringComparison.InvariantCultureIgnoreCase)).Value.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(authToken) && authToken.StartsWith("Bearer ", StringComparison.InvariantCultureIgnoreCase))
                {
                    authToken = authToken.Substring("Bearer ".Length);
                }
            }

            internal void InitializeClaims(ClaimsIdentity claimsIdentity)
            {
                askDelphiSystemID = claimsIdentity.FindFirst(Constants.ClaimTypeSystemID)?.Value;
                claims = claimsIdentity.Claims.Select(claim => new ClaimTuple { Type = claim.Type, Value = claim.Value });
            }

            public override string ToString()
            {
                return $"{GetType().Name}(id=\"{askDelphiSystemID}\", token=\"{authToken}\", claims={{{string.Join(",", claims)}}})";
            }
        }
    }
}
