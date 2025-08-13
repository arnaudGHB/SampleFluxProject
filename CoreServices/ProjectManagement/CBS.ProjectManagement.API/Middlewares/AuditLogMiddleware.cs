using System.Text;
using CBS.ProjectManagement.Data.Entity;
using CBS.ProjectManagement.Domain.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.ProjectManagement.API.Middlewares
{
    /// <summary>
    /// Middleware for logging audit information for all requests.
    /// </summary>
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLogMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditLogMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger.</param>
        public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware to log audit information for the current request.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="dbContext">The database context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context, ProjectContext dbContext)
        {
            // Skip logging for health check endpoints
            if (context.Request.Path.StartsWithSegments("/health") || 
                context.Request.Path.StartsWithSegments("/health-ui"))
            {
                await _next(context);
                return;
            }

            // Read the request body
            var request = await FormatRequest(context.Request);
            
            // Store the original response body stream
            var originalBodyStream = context.Response.Body;
            
            // Create a new memory stream for the response
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // Call the next middleware in the pipeline
                await _next(context);

                // Log the audit trail
                await LogAuditTrail(context, dbContext, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request");
                throw;
            }
            finally
            {
                // Copy the response body to the original stream
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

        private async Task LogAuditTrail(HttpContext context, ProjectContext dbContext, string request)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = context.User?.Identity?.Name ?? "Anonymous",
                    Action = $"{context.Request.Method} {context.Request.Path}",
                    TableName = context.Request.Path.Value?.Split('/').LastOrDefault() ?? "Unknown",
                    OldValues = string.Empty,
                    NewValues = request,
                    Timestamp = DateTime.UtcNow,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers["User-Agent"].ToString(),
                    StatusCode = context.Response.StatusCode.ToString(),
                    RequestUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}"
                };

                // Add the audit log to the database
                dbContext.AuditLogs.Add(auditLog);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the audit log");
                // Don't throw to avoid breaking the request pipeline
            }
        }
    }
}
