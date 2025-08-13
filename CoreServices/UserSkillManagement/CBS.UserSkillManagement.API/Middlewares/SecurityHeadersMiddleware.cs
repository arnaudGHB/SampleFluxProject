using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace CBS.UserSkillManagement.API
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers to the response
            context.Response.Headers.Add("X-Content-Type-Options", new StringValues("nosniff"));
            context.Response.Headers.Add("X-Frame-Options", new StringValues("DENY"));
            context.Response.Headers.Add("X-XSS-Protection", new StringValues("1; mode=block"));
            
            // Content Security Policy
            var csp = "default-src 'self'; " +
                     "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                     "style-src 'self' 'unsafe-inline'; " +
                     "img-src 'self' data:; " +
                     "font-src 'self';";
            
            context.Response.Headers.Add("Content-Security-Policy", new StringValues(csp));
            
            // Feature Policy
            var featurePolicy = "accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'";
            context.Response.Headers.Add("Feature-Policy", new StringValues(featurePolicy));
            
            // Referrer Policy
            context.Response.Headers.Add("Referrer-Policy", new StringValues("strict-origin-when-cross-origin"));
            
            // Permissions Policy
            context.Response.Headers.Add("Permissions-Policy", new StringValues("camera=(), geolocation=(), microphone=()"));

            await _next(context);
        }
    }

    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
