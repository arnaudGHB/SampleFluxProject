using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CBS.ProjectManagement.API.Middlewares
{
    /// <summary>
    /// Middleware for logging HTTP requests and responses.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestResponseLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger.</param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to process the HTTP request and log details.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Log the request
            var request = await FormatRequest(context.Request);
            _logger.LogInformation($"Request: {request}");

            // Copy the original response body stream
            var originalBodyStream = context.Response.Body;
            using (var responseBody = new MemoryStream())
            {
                // Replace the response body with our memory stream
                context.Response.Body = responseBody;

                // Call the next middleware in the pipeline
                await _next(context);

                // Log the response
                var response = await FormatResponse(context.Response);
                _logger.LogInformation($"Response: {response}");

                // Copy the contents of the new memory stream to the original stream
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private static async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0; // Reset the request body stream position

            return $"Method: {request.Method}, Path: {request.Path}, QueryString: {request.QueryString}, Body: {bodyAsText}";
        }

        private static async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"Status: {response.StatusCode}, Body: {text}";
        }
    }
}
