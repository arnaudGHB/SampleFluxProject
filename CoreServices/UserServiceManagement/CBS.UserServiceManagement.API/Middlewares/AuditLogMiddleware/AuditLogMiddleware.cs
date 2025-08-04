using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.API
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserContext dbContext)
        {
            context.Request.EnableBuffering();
            
            await _next(context);

            var request = context.Request;
            if (request.Method == "POST" || request.Method == "PUT" || request.Method == "DELETE")
            {
                if (context.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is string userId)
                {
                    var auditLog = new AuditLog
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        UserEmail = context.Items["Email"]?.ToString(),
                        EntityName = request.RouteValues["controller"]?.ToString(),
                        Action = request.Method,
                        Timestamp = DateTime.UtcNow,
                        Changes = await GetChangesAsync(request),
                        IPAddress = context.Connection.RemoteIpAddress?.ToString(),
                        Url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}"
                    };

                    dbContext.AuditLogs.Add(auditLog);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task<string> GetChangesAsync(HttpRequest request)
        {
            if (request.Method == "DELETE")
            {
                return $"Deleted resource with ID: {request.RouteValues["id"]?.ToString()}";
            }

            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true);
            var bodyAsText = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return bodyAsText;
        }
    }
}
