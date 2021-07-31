using AskDelphi.Adapter.AzureNuggets.DTO;
using AskDelphi.Adapter.AzureNuggets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.Controllers
{
    [Route("api/resources")]
    [ApiController]
    [Authorize]
    public class ResourcesController : ControllerBase
    {
        private readonly ILogger<ContentController> logger;
        private readonly IOperationContextFactory operationContextFactory;
        private readonly IResourcesService resourcesService;

        public ResourcesController(ILogger<ContentController> logger, IOperationContextFactory operationContextFactory, IResourcesService resourcesService)
        {
            this.logger = logger;
            this.operationContextFactory = operationContextFactory;
            this.resourcesService = resourcesService;
        }

        [HttpGet]
        [Route("list")]
        public async Task<JsonResult> GetResourcesList([FromQuery] string folderId = null)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetResourcesList)}()");

                operationContext.InitializeFromRequest(Request);

                ResourcesListSuccessResponse response = new ResourcesListSuccessResponse(operationContext);
                bool success = await resourcesService.List(operationContext, response, folderId);
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
        [Route("search")]
        public async Task<JsonResult> GetResourcesSearch([FromQuery] string query = null, [FromQuery] int page = 1, [FromQuery] int size = 100, [FromQuery] string continuationToken = null)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetResourcesSearch)}()");

                operationContext.InitializeFromRequest(Request);

                ResourcesSearchSuccessResponse response = new ResourcesSearchSuccessResponse(operationContext);
                bool success = await resourcesService.Search(operationContext, response, query, page, size, continuationToken);
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
        public async Task<JsonResult> GetResourcesMetadata([FromQuery] string resourceId)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetResourcesMetadata)}()");

                operationContext.InitializeFromRequest(Request);

                ResourcesMetadataSuccessResponse response = new ResourcesMetadataSuccessResponse(operationContext);
                bool success = await resourcesService.GetMetadata(operationContext, response, resourceId);
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

        [HttpHead]
        [Route("content")]
        public async Task HeadResourcesContent([FromQuery] string resourceId)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(HeadResourcesContent)}()");

                operationContext.InitializeFromRequest(Request);

                ResourceDescriptor resource = await resourcesService.GetResouceDescriptor(operationContext, resourceId);
                if (null != resource)
                {
                    Response.ContentLength = resource.ContentLength;
                    Response.ContentType = resource.MimeType;
                    Response.Headers.Add("Accept-Ranges", "bytes");
                    Response.StatusCode = StatusCodes.Status200OK;
                }
                else
                {
                    Response.StatusCode = StatusCodes.Status404NotFound;
                }
            }
        }

        [HttpGet]
        [Route("content")]
        public async Task<IActionResult> GetResourcesContent([FromQuery] string resourceId)
        {
            using (var operationContext = operationContextFactory.CreateOperationContext(HttpContext))
            {
                logger.LogInformation($"{operationContext.OperationId} := {nameof(GetResourcesContent)}()");

                operationContext.InitializeFromRequest(Request);

                ResourceDescriptor resource = await resourcesService.GetResouceDescriptor(operationContext, resourceId);
                if (null != resource)
                {
                    Stream contentStream = await resourcesService.GetResourceStream(operationContext, resourceId);
                    return File(contentStream, resource.MimeType, enableRangeProcessing: true);
                }
                else
                {
                    Response.StatusCode = StatusCodes.Status404NotFound;
                    return new JsonResult(new APIErrorResponse(operationContext, Constants.ErrorFailedCode, Constants.ErrorFailedMessage)); ;
                }
            }
        }
    }
}
