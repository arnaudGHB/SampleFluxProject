using CBS.AccountManagement.Data;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace  ReconciliationWorkerService
{
    /// <summary>
    /// Middleware for handling JSON Web Token (JWT) authentication.
    /// The JWTMiddleware class intercepts incoming HTTP requests, validates the JWT token,
    /// and attaches the authenticated user's information to the current HttpContext for
    /// further processing by the application.
    /// </summary>
    public static class JwtAuthenticationConfigurationExtension
    {
        public static void AddJwtAuthenticationConfiguration(
            this IServiceCollection services,
            JwtSettings settings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = settings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = settings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(settings.MinutesToExpiration)
                };
            });

            services.AddAuthorization();
        }
    }

}