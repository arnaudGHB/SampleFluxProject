using Microsoft.AspNetCore.Http;

namespace CBS.ProjectManagement.API.Middlewares
{
    /// <summary>
    /// Middleware for adding security-related HTTP headers.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityHeadersMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Invokes the middleware to add security headers to the HTTP response.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Add("Content-Security-Policy", 
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data:; " +
                "font-src 'self'; " +
                "connect-src 'self'; " +
                "frame-ancestors 'none';");
            
            // Remove the Server header
            context.Response.Headers.Remove("Server");
            
            // Call the next middleware in the pipeline
            await _next(context);
        }
    }
}
