using Polly;

namespace CBS.Gateway.API.Config
{
    // Custom middleware class responsible for enhancing security by adding security-related HTTP headers to the response.

    // Custom middleware class responsible for enhancing security by adding security-related HTTP headers to the response.
    public class CustomSecurityHeader
    {
        private readonly RequestDelegate _next;

        public CustomSecurityHeader(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var response = context.Response;

            // Control the Server and X-Powered-By response headers.
            // Control the Server response header.
            response.Headers.Remove("Server");
            response.Headers.Add("Server", "FLUX-POWER SERVER");

            // Control the X-Powered-By response header.
            response.Headers.Remove("X-Powered-By");
            response.Headers.Add("X-Powered-By", "FLUX SARL");
            // Control the other security-related response headers.
            response.Headers.Add("X-Frame-Options", "DENY");
            response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
            response.Headers.Add("X-Xss-Protection", "1; mode=block");
            response.Headers.Add("X-Content-Type-Options", "nosniff");
            response.Headers.Add("Referrer-Policy", "no-referrer");
            response.Headers.Add("Permissions-Policy", "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()");

            // Control the Content-Security-Policy response header.
            var contentSecurityPolicyHeaderKey = "Content-Security-Policy";
            var contentSecurityPolicyHeaderValue = "default-src 'self'; " +
                                                   "script-src 'self' 'unsafe-inline'; " +
                                                   "style-src 'self' 'unsafe-inline'; " +
                                                   "img-src 'self' data:; " +
                                                   "font-src 'self'; " +
                                                   "object-src 'none'; " +
                                                   "frame-ancestors 'none';";

            if (!response.Headers.ContainsKey(contentSecurityPolicyHeaderKey))
            {
                response.Headers.Add(contentSecurityPolicyHeaderKey, contentSecurityPolicyHeaderValue);
            }
            //        // Add HSTS (HTTP Strict Transport Security) header to enforce HTTPS.
            response.Headers.Add("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");
            await _next.Invoke(context);
        }
    }

    //public class CustomSecurityHeader
    //{
    //    private readonly RequestDelegate _next;

    //    public CustomSecurityHeader(RequestDelegate next)
    //    {
    //        _next = next;
    //    }

    //    public async Task Invoke(HttpContext context)
    //    {
    //        var response = context.Response;

    //        // Control the Server response header.
    //        response.Headers.Remove("Server");
    //        response.Headers.Add("Server", "FLUX-POWER SERVER");

    //        // Control the X-Powered-By response header.
    //        response.Headers.Remove("X-Powered-By");
    //        response.Headers.Add("X-Powered-By", "FLUX SAL");

    //        // Control other security-related response headers.
    //        response.Headers.Add("X-Frame-Options", "DENY");
    //        response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");
    //        response.Headers.Add("X-Xss-Protection", "1; mode=block");
    //        response.Headers.Add("X-Content-Type-Options", "nosniff");
    //        response.Headers.Add("Referrer-Policy", "no-referrer");
    //        response.Headers.Add("Permissions-Policy", "camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), usb=()");

    //        // Control the Content-Security-Policy response header.
    //        var contentSecurityPolicyHeaderKey = "Content-Security-Policy";
    //        var contentSecurityPolicyHeaderValue = "default-src 'self'; " +
    //                                               "script-src 'self' 'unsafe-inline'; " +
    //                                               "style-src 'self' 'unsafe-inline'; " +
    //                                               "img-src 'self' data:; " +
    //                                               "font-src 'self'; " +
    //                                               "object-src 'none'; " +
    //                                               "frame-ancestors 'none';";

    //        if (!response.Headers.ContainsKey(contentSecurityPolicyHeaderKey))
    //        {
    //            response.Headers.Add(contentSecurityPolicyHeaderKey, contentSecurityPolicyHeaderValue);
    //        }

    //        // Add HSTS (HTTP Strict Transport Security) header to enforce HTTPS.
    //        response.Headers.Add("Strict-Transport-Security", "max-age=63072000; includeSubDomains; preload");

    //        // Prevent MIME type sniffing.
    //        response.Headers.Add("X-Content-Type-Options", "nosniff");

    //        // Ensure that the header is added only once.
    //        context.Response.OnStarting(() =>
    //        {
    //            // Same-Origin Policy for iframe.
    //            if (!response.Headers.ContainsKey("X-Frame-Options"))
    //            {
    //                response.Headers.Add("X-Frame-Options", "DENY");
    //            }

    //            // Clickjacking protection.
    //            if (!response.Headers.ContainsKey("X-Frame-Options"))
    //            {
    //                response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    //            }

    //            // XSS protection.
    //            if (!response.Headers.ContainsKey("X-Xss-Protection"))
    //            {
    //                response.Headers.Add("X-Xss-Protection", "1; mode=block");
    //            }

    //            return Task.CompletedTask;
    //        });

    //        await _next.Invoke(context);
    //    }
    //}

}
