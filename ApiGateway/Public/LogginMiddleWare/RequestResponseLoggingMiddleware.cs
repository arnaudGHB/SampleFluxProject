using CBS.Gateway.DataContext;
using CBS.Gateway.DataContext.Repository.Uow;
using System.Text;

namespace CBS.Gateway.API.LogginMiddleWare
{

    public class RequestResponseLoggingMiddlewareOld
    {
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly PathHelper _pathHelper;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RequestResponseLoggingMiddlewareOld(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, PathHelper pathHelper, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            // Capture the correlation ID and IP address
            var correlationId = context.TraceIdentifier;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var sourceHost = context.Request.Host.ToString();
            var destinationHost = context.Request.Path;

            // Format request details
            var request = await FormatRequest(context.Request);
            var requestHeaders = FormatHeaders(context.Request.Headers);
            var requestBody = await FormatRequestBody(context.Request);

            // Store original response body stream
            var originalBodyStream = context.Response.Body;

            // Create new memory stream for response body
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // Continue processing request
                await _next(context);

                // Format response details
                var response = await FormatResponse(context.Response);
                var responseHeaders = FormatHeaders(context.Response.Headers);
                var responseBodyString = await FormatResponseBody(context.Response);

                // Log to file
                LogToFile(request, requestHeaders, requestBody, response, responseHeaders, responseBodyString, correlationId, ipAddress, sourceHost, destinationHost);

                // Log to database using scoped service
                await LogToDatabase(requestHeaders, requestBody, responseHeaders, responseBodyString, context, correlationId, ipAddress, sourceHost, destinationHost);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging request/response: {ex.Message}");
                await LogErrorToDatabase(requestHeaders, requestBody, context, correlationId, ipAddress, sourceHost, destinationHost, ex);
                throw; // Re-throw exception to propagate it further
            }
            finally
            {
                // Restore original response body stream for client
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = await FormatRequestBody(request);
            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {body}";
        }

        private async Task<string> FormatRequestBody(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var builder = new StringBuilder();
            foreach (var (key, value) in headers)
            {
                builder.Append($"{key}: {value}; ");
            }
            return builder.ToString();
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            return $"{response.StatusCode}: {await FormatResponseBody(response)}";
        }

        private async Task<string> FormatResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private async Task LogToDatabase(string requestHeaders, string requestBody, string responseHeaders, string responseBody, HttpContext context, string correlationId, string ipAddress, string sourceHost, string destinationHost)
        {
            // Use IServiceScopeFactory to create a scope and resolve scoped service (LoggingDbContext)
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();

                    // Log to database
                    var logEntry = new RequestResponseLog
                    {
                        Timestamp = DateTime.UtcNow,
                        RequestMethod = context.Request.Method,
                        RequestPath = context.Request.Path,
                        RequestHeaders = requestHeaders,
                        RequestBody = requestBody,
                        ResponseStatusCode = context.Response.StatusCode,
                        ResponseHeaders = responseHeaders,
                        ResponseBody = responseBody,
                        CorrelationId = correlationId,
                        IpAddress = ipAddress,
                        SourceHost = sourceHost,
                        DestinationHost = destinationHost
                    };

                    dbContext.RequestResponseLog.Add(logEntry);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging to database: {ex.Message}");
                throw; // Re-throw exception to propagate it further
            }
        }

        private async Task LogErrorToDatabase(string requestHeaders, string requestBody, HttpContext context, string correlationId, string ipAddress, string sourceHost, string destinationHost, Exception ex)
        {
            // Use IServiceScopeFactory to create a scope and resolve scoped service (LoggingDbContext)
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();

                    // Log error to database
                    var logEntry = new RequestResponseLog
                    {
                        Timestamp = DateTime.UtcNow,
                        RequestMethod = context.Request.Method,
                        RequestPath = context.Request.Path,
                        RequestHeaders = requestHeaders,
                        RequestBody = requestBody,
                        ResponseStatusCode = 500,
                        ResponseHeaders = string.Empty,
                        ResponseBody = string.Empty,
                        CorrelationId = correlationId,
                        IpAddress = ipAddress,
                        SourceHost = sourceHost,
                        DestinationHost = destinationHost,
                        ErrorDetails = ex.ToString()
                    };

                    dbContext.RequestResponseLog.Add(logEntry);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception innerEx)
            {
                _logger.LogError($"Error logging error to database: {innerEx.Message}");
                throw; // Re-throw exception to propagate it further
            }
        }

        private void LogToFile(string request, string requestHeaders, string requestBody, string response, string responseHeaders, string responseBody, string correlationId, string ipAddress, string sourceHost, string destinationHost)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Request:");
            builder.AppendLine(request);
            builder.AppendLine("Request Headers:");
            builder.AppendLine(requestHeaders);
            builder.AppendLine("Request Body:");
            builder.AppendLine(requestBody);
            builder.AppendLine("Response:");
            builder.AppendLine(response);
            builder.AppendLine("Response Headers:");
            builder.AppendLine(responseHeaders);
            builder.AppendLine("Response Body:");
            builder.AppendLine(responseBody);
            builder.AppendLine($"Correlation ID: {correlationId}");
            builder.AppendLine($"IP Address: {ipAddress}");
            builder.AppendLine($"Source Host: {sourceHost}");
            builder.AppendLine($"Destination Host: {destinationHost}");

            // Ensure the log directory exists
            if (!Directory.Exists(_pathHelper.LoggerPath))
            {
                Directory.CreateDirectory(_pathHelper.LoggerPath);
            }

            // Log to file
            var logFilePath = Path.Combine(_pathHelper.LoggerPath, $"Log_{DateTime.Now.ToString("yyyyMMdd_hh")}.log");
            File.AppendAllText(logFilePath, builder.ToString());
        }
    }

    public class RequestResponseLoggingMiddleware
    {
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly PathHelper _pathHelper;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, PathHelper pathHelper, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            // Capture the correlation ID and IP address
            var correlationId = context.TraceIdentifier;
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var sourceHost = context.Request.Host.ToString();
            var destinationHost = context.Request.Path;

            // Format request details
            var request = await FormatRequest(context.Request);
            var requestHeaders = FormatHeaders(context.Request.Headers);
            var requestBody = await FormatRequestBody(context.Request);

            // Store original response body stream
            var originalBodyStream = context.Response.Body;

            // Create new memory stream for response body
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                // Continue processing request
                await _next(context);

                // Format response details
                var response = await FormatResponse(context.Response);
                var responseHeaders = FormatHeaders(context.Response.Headers);
                var responseBodyString = await FormatResponseBody(context.Response);

                // Log to file
                LogToFile(request, requestHeaders, requestBody, response, responseHeaders, responseBodyString, correlationId, ipAddress, sourceHost, destinationHost);

                // Log to database using scoped service
                await LogToDatabase(requestHeaders, requestBody, responseHeaders, responseBodyString, context, correlationId, ipAddress, sourceHost, destinationHost);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging request/response: {ex.Message}");
                await LogErrorToDatabase(requestHeaders, requestBody, context, correlationId, ipAddress, sourceHost, destinationHost, ex);
                throw; // Re-throw exception to propagate it further
            }
            finally
            {
                // Restore original response body stream for client
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var body = await FormatRequestBody(request);
            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {body}";
        }

        private async Task<string> FormatRequestBody(HttpRequest request)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private string FormatHeaders(IHeaderDictionary headers)
        {
            var builder = new StringBuilder();
            foreach (var (key, value) in headers)
            {
                builder.Append($"{key}: {value}; ");
            }
            return builder.ToString();
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            return $"{response.StatusCode}: {await FormatResponseBody(response)}";
        }

        private async Task<string> FormatResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }

        private async Task LogToDatabase(
    string requestHeaders,
    string requestBody,
    string responseHeaders,
    string responseBody,
    HttpContext context,
    string correlationId,
    string ipAddress,
    string sourceHost,
    string destinationHost)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var mongoUnitOfWork = scope.ServiceProvider.GetRequiredService<IMongoUnitOfWork>();
                    var logRepository = mongoUnitOfWork.GetRepository<RequestResponseLog>();

                    var logEntry = new RequestResponseLog
                    {
                        Timestamp = DateTime.UtcNow,
                        RequestMethod = context.Request.Method,
                        RequestPath = context.Request.Path,
                        RequestHeaders = requestHeaders,
                        RequestBody = requestBody,
                        ResponseStatusCode = context.Response.StatusCode,
                        ResponseHeaders = responseHeaders,
                        ResponseBody = responseBody,
                        CorrelationId = correlationId,
                        IpAddress = ipAddress,
                        SourceHost = sourceHost,
                        DestinationHost = destinationHost
                    };

                    await logRepository.InsertAsync(logEntry);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging to MongoDB: {ex.Message}");
                throw;
            }
        }

        private async Task LogErrorToDatabase(
            string requestHeaders,
            string requestBody,
            HttpContext context,
            string correlationId,
            string ipAddress,
            string sourceHost,
            string destinationHost,
            Exception ex)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var mongoUnitOfWork = scope.ServiceProvider.GetRequiredService<IMongoUnitOfWork>();
                    var logRepository = mongoUnitOfWork.GetRepository<RequestResponseLog>();

                    var errorLogEntry = new RequestResponseLog
                    {
                        Timestamp = DateTime.UtcNow,
                        RequestMethod = context.Request.Method,
                        RequestPath = context.Request.Path,
                        RequestHeaders = requestHeaders,
                        RequestBody = requestBody,
                        ResponseStatusCode = 500,
                        ResponseHeaders = string.Empty,
                        ResponseBody = string.Empty,
                        CorrelationId = correlationId,
                        IpAddress = ipAddress,
                        SourceHost = sourceHost,
                        DestinationHost = destinationHost,
                        ErrorDetails = ex.ToString()
                    };

                    await logRepository.InsertAsync(errorLogEntry);
                }
            }
            catch (Exception innerEx)
            {
                _logger.LogError($"Error logging error to MongoDB: {innerEx.Message}");
                throw;
            }
        }


        private void LogToFile(string request, string requestHeaders, string requestBody, string response, string responseHeaders, string responseBody, string correlationId, string ipAddress, string sourceHost, string destinationHost)
        {
            var builder = new StringBuilder();
            builder.AppendLine("Request:");
            builder.AppendLine(request);
            builder.AppendLine("Request Headers:");
            builder.AppendLine(requestHeaders);
            builder.AppendLine("Request Body:");
            builder.AppendLine(requestBody);
            builder.AppendLine("Response:");
            builder.AppendLine(response);
            builder.AppendLine("Response Headers:");
            builder.AppendLine(responseHeaders);
            builder.AppendLine("Response Body:");
            builder.AppendLine(responseBody);
            builder.AppendLine($"Correlation ID: {correlationId}");
            builder.AppendLine($"IP Address: {ipAddress}");
            builder.AppendLine($"Source Host: {sourceHost}");
            builder.AppendLine($"Destination Host: {destinationHost}");

            // Ensure the log directory exists
            if (!Directory.Exists(_pathHelper.LoggerPath))
            {
                Directory.CreateDirectory(_pathHelper.LoggerPath);
            }

            // Log to file
            var logFilePath = Path.Combine(_pathHelper.LoggerPath, $"Log_{DateTime.Now.ToString("yyyyMMdd_hh")}.log");
            File.AppendAllText(logFilePath, builder.ToString());
        }
    }

}
