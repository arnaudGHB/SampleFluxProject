using CBS.Gateway.API.JTWMiddleware.CustomerJWTValidator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;


namespace CBS.Gateway.API.Middleware
{



    //public class TokenValidationMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly IOcelotLogger _logger;
    //    private readonly IConfiguration _configuration;
    //    public TokenValidationMiddleware(RequestDelegate next, IOcelotLoggerFactory loggerFactory, IConfiguration configuration)
    //    {
    //        _next = next;
    //        _logger = loggerFactory.CreateLogger<TokenValidationMiddleware>();
    //        _configuration = configuration;
    //    }

    //    public async Task InvokeAsync(HttpContext context)
    //    {
    //        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

    //        if (string.IsNullOrEmpty(token))
    //        {
    //            context.Response.StatusCode = 401;
    //            await context.Response.WriteAsync("Unauthorized: Missing token");
    //            return;
    //        }

    //        // Specify your authentication server's key and issuer for token validation
            

    //        try
    //        {
    //            var tokenHandler = new JwtSecurityTokenHandler();
    //            var validationParameters = new TokenValidationParameters
    //            {
    //                ValidateIssuerSigningKey = true,
    //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])),
    //                ValidateIssuer = true,
    //                ValidIssuer = _configuration["Jwt:ValidIssuer"],
    //                ValidateAudience = true,
    //                ValidAudience = _configuration["Jwt:ValidAudience"],
    //                ValidateLifetime = true,
    //                ClockSkew = TimeSpan.FromMinutes(Convert.ToInt32(_configuration["Jwt:TimeInHours"]))

    //            };
    //            var customClockSkew = TimeSpan.FromMinutes(60); // Set a negative custom clock skew (e.g., 30 minutes behind current time)
    //            var customTokenHandler = new CustomJwtSecurityTokenHandler(customClockSkew);

    //            var principal = customTokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
    //            context.User = principal;

    //            await _next.Invoke(context);

 
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogError($"Token validation failed, {ex.Message}", ex);
    //            context.Response.StatusCode = 401;
    //            await context.Response.WriteAsync("Unauthorized: Invalid token");
    //        }

    //    }
    //    /// <summary>
    //    /// Checks if the provided JWT token is expired.
    //    /// </summary>
    //    /// <param name="token">The JWT token to be checked.</param>
    //    /// <returns>True if the token is expired, false otherwise.</returns>
    //    public static bool IsTokenExpired(string token)
    //    {
    //        try
    //        {
    //            var handler = new JwtSecurityTokenHandler();
    //            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

    //            if (jsonToken == null)
    //                return true;

    //            var expiryTime = jsonToken.ValidTo;
    //            if (expiryTime < DateTime.UtcNow)
    //            {
    //                // Token is expired
    //                return true;
    //            }

    //            return false;
    //        }
    //        catch (Exception)
    //        {
    //            // Token is invalid or expired
    //            return true;
    //        }
    //    }
    //}
   

    //public static class TokenValidationMiddlewareExtensions
    //{
    //    public static IServiceCollection AddTokenValidationMiddleware(this IServiceCollection services)
    //    {
    //        services.AddSingleton<TokenValidationMiddleware>();
    //        return services;
    //    }
    //}



    /// <summary>
    /// Middleware for handling JSON Web Token (JWT) authentication.
    /// The JWTMiddleware class intercepts incoming HTTP requests, validates the JWT token,
    /// and attaches the authenticated user's information to the current HttpContext for
    /// further processing by the application.
    /// </summary>
    //public class JWTMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly IConfiguration _configuration;

    //    /// <summary>
    //    /// Initializes a new instance of the JWTMiddleware class.
    //    /// </summary>
    //    /// <param name="next">The next middleware component in the pipeline.</param>
    //    /// <param name="configuration">The configuration containing JWT-related settings.</param>
    //    public JWTMiddleware(RequestDelegate next, IConfiguration configuration)
    //    {
    //        _next = next ?? throw new ArgumentNullException(nameof(next));
    //        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    //    }

    //    /// <summary>
    //    /// Invokes the middleware to handle the incoming HTTP request.
    //    /// Validates the JWT token and attaches user information to the HttpContext.
    //    /// </summary>
    //    /// <param name="context">The current HttpContext for the request.</param>
    //    public async Task Invoke(HttpContext context)
    //    {
    //        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

    //        if (token != null)
    //            AttachAccountToContext(context, token);

    //        await _next(context);
    //    }

    //    /// <summary>
    //    /// Checks if the provided JWT token is expired.
    //    /// </summary>
    //    /// <param name="token">The JWT token to be checked.</param>
    //    /// <returns>True if the token is expired, false otherwise.</returns>
    //    public static bool IsTokenExpired(string token)
    //    {
    //        try
    //        {
    //            var handler = new JwtSecurityTokenHandler();
    //            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

    //            if (jsonToken == null)
    //                return true;

    //            var expiryTime = jsonToken.ValidTo;
    //            if (expiryTime < DateTime.UtcNow)
    //            {
    //                // Token is expired
    //                return true;
    //            }

    //            return false;
    //        }
    //        catch (Exception)
    //        {
    //            // Token is invalid or expired
    //            return true;
    //        }
    //    }

    //    /// <summary>
    //    /// Validates the JWT token, extracts user information, and attaches it to the HttpContext.
    //    /// </summary>
    //    /// <param name="context">The current HttpContext for the request.</param>
    //    /// <param name="token">The JWT token to be validated and processed.</param>
    //    private void AttachAccountToContext(HttpContext context, string token)
    //    {
    //        try
    //        {
    //            if (!IsTokenExpired(token))
    //            {
    //                var tokenHandler = new JwtSecurityTokenHandler();
    //                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"]);
    //                tokenHandler.ValidateToken(token, new TokenValidationParameters
    //                {
    //                    ValidateIssuerSigningKey = true,
    //                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"])),
    //                    ValidateIssuer = true,
    //                    ValidIssuer = _configuration["Jwt:ValidIssuer"],
    //                    ValidateAudience = true,
    //                    ValidAudience = _configuration["Jwt:ValidAudience"],
    //                    ValidateLifetime = true,
    //                    ClockSkew = TimeSpan.FromMinutes(Convert.ToInt32(_configuration["Jwt:TimeInHours"]))
    //                }, out SecurityToken validatedToken);

    //                var jwtToken = (JwtSecurityToken)validatedToken;
    //                var accountId = new Guid(jwtToken.Claims.First(x => x.Type == "Id").Value);
    //                var userName = jwtToken.Claims.First(x => x.Type == "UserName").Value;
    //                var email = jwtToken.Claims.First(x => x.Type == "Email").Value;

    //                // Attach account information to the context on successful JWT validation
    //                context.Items["accountms"] = token;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.WriteLine(ex.Message);
    //            // Do nothing if JWT validation fails
    //            // Account is not attached to context, so the request won't have access to secure routes
    //        }
    //    }
    //}
}
