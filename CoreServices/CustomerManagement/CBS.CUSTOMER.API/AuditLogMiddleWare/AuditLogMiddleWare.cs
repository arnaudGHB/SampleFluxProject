using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DOMAIN.Context;
using System.Text;

namespace CBS.CUSTOMER.API
{
    // Middleware designed to audit API requests by logging them into a database
    public class AuditLogMiddleware
    {
        // Constants for keys used in requests and data
        private const string ControllerKey = "controller";
        private const string IdKey = "id";

        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Invokes the middleware to handle logging before and after request execution
        public async Task InvokeAsync(HttpContext context, POSContext dbContext)
        {
            await _next(context); // Executes the request pipeline

            var request = context.Request;
            var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            // Logs API requests
            if (request.Path.StartsWithSegments("/api"))
            {
                request.RouteValues.TryGetValue(ControllerKey, out var controllerValue);
                var controllerName = (string)(controllerValue ?? string.Empty);
                var changedValue = await GetChangedValues(request).ConfigureAwait(false);
                // Logs an audit trail for authenticated users
                if (request.HttpContext.User.Identity.IsAuthenticated)
                {
                    var auditLog = new AuditLog
                    {
                        EntityName = controllerName,
                        Action = request.Method,
                        Timestamp = DateTime.UtcNow,
                        Changes = changedValue,
                        UserId = context.Items["CustomerId"]?.ToString(),
                        UserEmail = context.Items["Email"]?.ToString(),
                        IPAddress = context.Connection.RemoteIpAddress?.ToString(),
                        Url = url,
                    };
                    dbContext.AuditLogs.Add(auditLog);
                    await dbContext.SaveChangesAsync();
                }
                // Logs an audit trail for unauthenticated users
                else
                {
                    var auditLog = new AuditLog
                    {
                        EntityName = controllerName,
                        Action = $"No token was found. Method: {request.Method}",
                        Timestamp = DateTime.UtcNow,
                        Changes = changedValue,
                        UserId = string.Empty,
                        UserEmail = string.Empty,
                        IPAddress = context.Connection.RemoteIpAddress.ToString(),
                        Url = url,
                    };
                    dbContext.AuditLogs.Add(auditLog);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        // Retrieves changed values based on different HTTP request methods
        private static async Task<string> GetChangedValues(HttpRequest request)
        {
            var changedValue = string.Empty;

            switch (request.Method)
            {
                case "POST":
                case "PUT":
                    changedValue = await ReadRequestBody(request, Encoding.UTF8).ConfigureAwait(false);
                    break;

                case "DELETE":
                    request.RouteValues.TryGetValue(IdKey, out var idValueObj);
                    changedValue = (string?)idValueObj ?? string.Empty;
                    break;

                default:
                    break;
            }

            return changedValue;
        }

        // Reads and retrieves request body content
        private static async Task<string> ReadRequestBody(HttpRequest request, Encoding? encoding = null)
        {
            request.Body.Position = 0;
            var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);
            var requestBody = await reader.ReadToEndAsync().ConfigureAwait(false);
            request.Body.Position = 0;

            return requestBody;
        }
    }
}