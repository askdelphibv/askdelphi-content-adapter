using AskDelphi.Adapter.AzureNuggets.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public static readonly TimeSpan TokenLifetime = TimeSpan.FromMinutes(60);

        public TokenService(ILogger<TokenService> logger, IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> GenerateToken(IOperationContext operationContext, string purpose, ClaimTuple[] claims)
        {
            string cfgSecret = configuration.GetValue<string>("JWT:Secret");
            string cfgIssuer = configuration.GetValue<string>("JWT:Issuer");
            string cfgAudience = configuration.GetValue<string>("JWT:Audience");
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(cfgSecret));

            var tokenHandler = new JwtSecurityTokenHandler();
            DateTime tokenExpirationTime = DateTime.UtcNow + TokenLifetime;

            List<Claim> tokenClaims = new List<Claim>();
            tokenClaims.Add(new Claim(ClaimTypes.NameIdentifier, purpose));
            tokenClaims.AddRange((claims ?? new ClaimTuple[] { }).Select(claim => new Claim(claim.Type, claim.Value)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(tokenClaims),
                Expires = tokenExpirationTime,
                Issuer = cfgIssuer,
                Audience = cfgAudience,
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string result = tokenHandler.WriteToken(token);
            return await Task.FromResult<string>(result);
        }

        public async Task InvalidateToken(IOperationContext operationContext)
        {
            Trace.TraceInformation($"{operationContext.OperationId} TokenService.InvalidateToken() was requested, but this is not implemented. The token will remain active for the remainder of its lifetime.");
            // We're choosing to not invalidate the token globally for this PoC. Instead, token lifetime is limited causing tokens to be reasonably short-lived.
            // If a higher-level of security is required, this would be the correct place to register the token in a shared REDIS cache for the remainder of its lifetime.
            await Task.FromResult(0);
        }

        public async Task<string> Refresh(IOperationContext operationContext, string refreshToken)
        {
            Trace.TraceInformation($"{operationContext.OperationId} TokenService.Refresh() was requested, but this is not implemented, returning null to indicate the caller should simply log in again.");
            return await Task.FromResult<string>(null);
        }

        public async Task<bool> ValidateToken(IOperationContext operationContext)
        {
            string token = operationContext.GetAuthToken();

            string cfgSecret = configuration.GetValue<string>("JWT:Secret");
            string cfgIssuer = configuration.GetValue<string>("JWT:Issuer");
            string cfgAudience = configuration.GetValue<string>("JWT:Audience");
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(cfgSecret));

            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = cfgIssuer,
                    ValidAudience = cfgAudience,
                    IssuerSigningKey = securityKey
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return await Task.FromResult<bool>(true);
        }
    }
}
