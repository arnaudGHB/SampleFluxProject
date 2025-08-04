using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using System.Text;

namespace CBS.BankMGT.API
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

        public async Task InvokeAsync(HttpContext context, POSContext dbContext)
        {
            try
            {
                // Enable buffering to allow the request body to be read multiple times
                context.Request.EnableBuffering();

                await _next(context); // Executes the request pipeline
                await LogAuditTrailAsync(context, dbContext);
            }
            catch (Exception e)
            {
                await LogErrorAuditTrailAsync(context, dbContext, e);
            }
        }

        private async Task LogAuditTrailAsync(HttpContext context, POSContext dbContext)
        {
            try
            {
                var request = context.Request;
                var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

                // Logs API requests
                if (request.Path.StartsWithSegments("/api"))
                {
                    request.RouteValues.TryGetValue(ControllerKey, out var controllerValue);
                    var controllerName = (string)(controllerValue ?? string.Empty);
                    var changedValue = await GetChangedValuesAsync(request);

                    // Logs an audit trail for authenticated users
                    if (context.User.Identity.IsAuthenticated)
                    {
                        var auditLog = CreateAuditLog(controllerName, request, changedValue, context, url);
                        dbContext.AuditLogs.Add(auditLog);
                        await dbContext.SaveChangesAsync();
                    }
                    // Logs an audit trail for unauthenticated users
                    else
                    {
                        var auditLog = CreateAuditLog(controllerName, request, changedValue, context, url, true);
                        dbContext.AuditLogs.Add(auditLog);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw;
            }
        }

        private async Task LogErrorAuditTrailAsync(HttpContext context, POSContext dbContext, Exception e)
        {
            var request = context.Request;
            request.RouteValues.TryGetValue(ControllerKey, out var controllerValue);
            var controllerName = (string)(controllerValue ?? string.Empty);
            var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            var auditLog = CreateAuditLog(controllerName, request, $"Error occurred: {request.Method}, Error: {e.Message}", context, url);
            dbContext.AuditLogs.Add(auditLog);
            await dbContext.SaveChangesAsync();
        }

        private AuditLog CreateAuditLog(string controllerName, HttpRequest request, string action, HttpContext context, string url, bool isUnauthenticated = false)
        {
            return new AuditLog
            {
                EntityName = controllerName,
                Action = isUnauthenticated ? action : request.Method,
                Timestamp = DateTime.UtcNow,
                Changes = isUnauthenticated ? string.Empty : GetChangedValuesAsync(request).GetAwaiter().GetResult(),
                UserId = context.Items["Id"]?.ToString(),
                UserEmail = context.Items["Email"]?.ToString(),
                IPAddress = GetClientIpAddress(context),
                Url = url,
            };
        }

        private string GetClientIpAddress(HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private async Task<string> GetChangedValuesAsync(HttpRequest request)
        {
            var changedValue = string.Empty;

            switch (request.Method)
            {
                case "POST":
                case "PUT":
                    changedValue = await ReadRequestBodyAsync(request, Encoding.UTF8);
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

        private async Task<string> ReadRequestBodyAsync(HttpRequest request, Encoding encoding)
        {
            // Reset the stream position to the beginning
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, encoding, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            // Reset the stream position to the beginning again for next readers
            request.Body.Position = 0;
            return body;
        }
    }
}
