using CBS.NLoan.Helper.Helper;
using System.Text;

namespace CBS.NLoan.API
{
    /// <summary>
    /// Middleware for logging incoming requests and outgoing responses.
    /// The RequestResponseLoggingMiddleware class intercepts requests and responses, logs their details, and passes them to the next middleware.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the RequestResponseLoggingMiddleware.
        /// </summary>
        /// <param name="next">Next middleware in the pipeline.</param>
        /// <param name="logger">Logger for logging request and response details.</param>
        /// <param name="pathHelper">Helper class for managing file paths.</param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger, PathHelper pathHelper)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));
        }

        /// <summary>
        /// Invokes the middleware to log request and response details.
        /// </summary>
        /// <param name="context">HTTP context containing request and response information.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            var builder = new StringBuilder();

            // Format and log the incoming request details
            var request = await FormatRequest(context.Request);
            builder.Append("Request: ").AppendLine(request);
            builder.AppendLine("Request headers:");
            foreach (var header in context.Request.Headers)
            {
                builder.Append(header.Key).Append(':').AppendLine(header.Value);
            }

            // Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            // Create a new memory stream and use it for the temporary response body
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Continue down the Middleware pipeline, eventually returning to this class
            await _next(context);

            // Format and log the outgoing response details
            var response = await FormatResponse(context.Response);
            builder.Append("Response: ").AppendLine(response);
            builder.AppendLine("Response headers: ");
            foreach (var header in context.Response.Headers)
            {
                builder.Append(header.Key).Append(':').AppendLine(header.Value);
            }

            // Create the log directory if it does not exist
            if (!Directory.Exists(_pathHelper.LoggerPath))
            {
                Directory.CreateDirectory(_pathHelper.LoggerPath);
            }

            // Save log to the chosen datastore
            _logger.LogInformation(builder.ToString());

            // Copy the contents of the new memory stream (which contains the response) to the original stream, returned to the client
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            // Leave the body open so the next middleware can read it
            using var reader = new StreamReader(
                request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            // Format the request details
            var formattedRequest = $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {body}";

            // Reset the request body stream position so the next middleware can read it
            request.Body.Position = 0;

            return formattedRequest;
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            // We need to read the response stream from the beginning
            response.Body.Seek(0, SeekOrigin.Begin);

            // Copy the response stream into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            // Reset the reader for the response so the client can read it
            response.Body.Seek(0, SeekOrigin.Begin);

            // Return the string for the response, including the Status code (e.g., 200, 404, 401, etc.)
            return $"{response.StatusCode}: {text}";
        }
    }
}