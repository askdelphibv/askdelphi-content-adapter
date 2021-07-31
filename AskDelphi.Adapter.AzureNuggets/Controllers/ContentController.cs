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
    [Route("api/content")]
    [ApiController]
    [Authorize]
    public class ContentController : ControllerBase
    {
        private readonly ILogger<ContentController> logger;
        private readonly IOperationContextFactory operationContextFactory;
        private readonly IContentService contentService;

        public ContentController(ILogger<ContentController> logger, IOperationContextFactory operationContextFactory, IContentService contentService)
        {
            this.logger = logger;
            this.operationContextFactory = operationContextFactory;
            this.contentService = contentService;
        }

        [HttpGet]
        [Route("folders")]
        public async Task<JsonResult> GetFolders([FromQuery] string folderId = null)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetFolders)}()");

                operationContext.InitializeFromRequest(Request);

                ContentFoldersSuccessResponse response = new ContentFoldersSuccessResponse(operationContext);
                bool success = await contentService.ListFolders(operationContext, response, folderId);
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

        [HttpPost]
        [Route("search")]
        public async Task<JsonResult> PostSearch([FromBody]ContentSearchRequest request)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(PostSearch)}()");

                operationContext.InitializeFromRequest(Request);

                ContentSearchSuccessResponse response = new ContentSearchSuccessResponse(operationContext);
                bool success = await contentService.Search(operationContext, response, request);
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
        [Route("metadata")]
        public async Task<JsonResult> GetMetadata([FromQuery] string topicId)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetMetadata)}()");

                operationContext.InitializeFromRequest(Request);

                ContentMetadataSuccessResponse response = new ContentMetadataSuccessResponse(operationContext);
                bool success = await contentService.GetMetadata(operationContext, response, topicId);
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
        [Route("content")]
        public async Task<JsonResult> GetContent([FromQuery] string topicId)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetContent)}()");

                operationContext.InitializeFromRequest(Request);

                ContentContentSuccessResponse response = new ContentContentSuccessResponse(operationContext);
                bool success = await contentService.GetContent(operationContext, response, topicId);
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
    }
}
