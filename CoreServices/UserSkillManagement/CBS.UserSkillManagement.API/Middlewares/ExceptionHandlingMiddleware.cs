using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CBS.UserSkillManagement.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CBS.UserSkillManagement.API
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;
            var errorResponse = new ServiceResponse<object>();
            
            switch (exception)
            {
                case ApplicationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = ServiceResponse<object>.ErrorResponse(ex.Message);
                    break;
                case UnauthorizedAccessException ex:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = ServiceResponse<object>.ErrorResponse("Unauthorized Access");
                    break;
                case KeyNotFoundException ex:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = ServiceResponse<object>.ErrorResponse("Resource not found");
                    break;
                default:
                    // Log unhandled exceptions
                    _logger.LogError(exception, "An unhandled exception has occurred");
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = ServiceResponse<object>.ErrorResponse("Internal server error. Please try again later.");
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
