using AskDelphi.Adapter.AzureNuggets.DTO;
using AskDelphi.Adapter.AzureNuggets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> logger;
        private readonly IOperationContextFactory operationContextFactory;
        private readonly IAuthenticationService authenticationService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IOperationContextFactory operationContextFactory, IAuthenticationService authenticationService)
        {
            this.logger = logger;
            this.operationContextFactory = operationContextFactory;
            this.authenticationService = authenticationService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<JsonResult> PostAuthLogin([FromBody(EmptyBodyBehavior = Microsoft.AspNetCore.Mvc.ModelBinding.EmptyBodyBehavior.Allow)] ClaimTuple[] claims = null)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(PostAuthLogin)}()");
                var authHeader = Request.Headers.FirstOrDefault(x => string.Equals(x.Key, "Authorization", StringComparison.InvariantCultureIgnoreCase));
                operationContext.InitializeFromRequest(Request);

                AuthLoginSuccessResponse response = new AuthLoginSuccessResponse(operationContext);
                bool success = await authenticationService.Login(operationContext, response, authHeader, claims);
                if (success)
                {
                    Response.StatusCode = StatusCodes.Status200OK;
                    return new JsonResult(response);
                }
                else
                {
                    var error = new APIErrorResponse(operationContext, Constants.ErrorInvalidCredentialsCode, Constants.ErrorInvalidCredentialsMessage);
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return new JsonResult(error);
                }
            }
        }

        [HttpGet]
        [Route("logout")]
        [Authorize]
        public async Task<JsonResult> GetAuthLogout()
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetAuthLogout)}()");

                operationContext.InitializeFromRequest(Request);

                AuthLogoutSuccessResponse response = new AuthLogoutSuccessResponse(operationContext);
                bool success = await authenticationService.Logout(operationContext, response);
                if (success)
                {
                    Response.StatusCode = StatusCodes.Status200OK;
                    return new JsonResult(response);
                }
                else
                {
                    var error = new APIErrorResponse(operationContext, Constants.ErrorFailedCode, Constants.ErrorFailedMessage);
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return new JsonResult(error);
                }
            }
        }

        [HttpGet]
        [Route("refresh")]
        [Authorize]
        public async Task<JsonResult> GetAuthRefresh([FromQuery] string refresh)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetAuthRefresh)}()");

                operationContext.InitializeFromRequest(Request);

                AuthRefreshSuccessResponse response = new AuthRefreshSuccessResponse(operationContext);
                bool success = await authenticationService.Refresh(operationContext, response, refresh);
                if (success)
                {
                    Response.StatusCode = StatusCodes.Status200OK;
                    return new JsonResult(response);
                }
                else
                {
                    var error = new APIErrorResponse(operationContext, Constants.ErrorNotSupportedCode, Constants.ErrorNotSupportedMessage);
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return new JsonResult(error);
                }
            }
        }
    }
}
