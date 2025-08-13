using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.UserSkillManagement.API
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip logging for health checks and swagger
            if (context.Request.Path.StartsWithSegments("/health") || 
                context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Log the request
            var request = await FormatRequest(context.Request);
            _logger.LogInformation($"Request: {request}");

            // Copy the original response body stream
            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    await _next(context);
                    stopwatch.Stop();

                    // Log the response
                    var response = await FormatResponse(context.Response);
                    _logger.LogInformation($"Response: {response} | Duration: {stopwatch.ElapsedMilliseconds}ms");

                    await responseBody.CopyToAsync(originalBodyStream);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger.LogError(ex, $"An error occurred while processing the request. Duration: {stopwatch.ElapsedMilliseconds}ms");
                    throw;
                }
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var body = request.Body;
            var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Position = 0;

            var requestInfo = new
            {
                Method = request.Method,
                Path = request.Path,
                QueryString = request.QueryString,
                Headers = request.Headers,
                Body = bodyAsText
            };

            return JsonConvert.SerializeObject(requestInfo, Formatting.Indented);
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            var responseInfo = new
            {
                StatusCode = response.StatusCode,
                Headers = response.Headers,
                Body = text.Length > 1000 ? "[Response body too large to log]" : text
            };

            return JsonConvert.SerializeObject(responseInfo, Formatting.Indented);
        }
    }

    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestResponseLoggingMiddleware>();
        }
    }
}
