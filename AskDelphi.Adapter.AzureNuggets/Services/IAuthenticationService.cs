using AskDelphi.Adapter.AzureNuggets.DTO;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Services
{
    /// <summary>
    /// Authentication services.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Requests a session to be logged in. Upon success, will update the response object with a JWT object containing information about the logged-in session.
        /// </summary>
        /// <remarks>
        /// The authorization header will have to contain the value Bearer {login}:{password}. The login-password options supported are stored as Keys:{login} = {password} properties in the application configuration.
        /// </remarks>
        /// <param name="operationContext">Context for the operation.</param>
        /// <param name="response">Response object that can be returned on the API.</param>
        /// <param name="authHeader">The authentication header containing the Authorization header value for the login request.</param>
        /// <param name="claims">Optional list of additional claims that can be requested to be included in the resulting JWT object. This can be used to encode additional information that is relevant to the current session and can be communicated to all other operations via the claims. The caller of this API will always include a special claim of type http://tempuri.org/askdelphi/remote-system-id with a string value. This special claim provides the system with an identification of itself inside the AskDelphi environment.</param>
        /// <returns>true upon success, false otherwise</returns>
        Task<bool> Login(IOperationContext operationContext, AuthLoginSuccessResponse response, KeyValuePair<string, StringValues> authHeader, ClaimTuple[] claims);

        /// <summary>
        /// Requests the session to be logged out.
        /// </summary>
        /// <param name="operationContext">Operation context containing the session information.</param>
        /// <param name="response">Response object that can be returned on the API.</param>
        /// <returns>true upon success, false otherwise</returns>
        Task<bool> Logout(IOperationContext operationContext, AuthLogoutSuccessResponse response);

        /// <summary>
        /// Requests a new JWT object to be created for the same session that's currently used. Implementing this method is optional. If not implemented, the method will return a null-token and return false.
        /// </summary>
        /// <param name="operationContext">Operation context containing the session information.</param>
        /// <param name="response">Response object that can be returned on the API.</param>
        /// <param name="refreshToken">The refresh token that was generated when the JWT object was first returned.</param>
        /// <returns>true upon success, false otherwise</returns>
        Task<bool> Refresh(IOperationContext operationContext, AuthRefreshSuccessResponse response, string refreshToken);
    }
}
