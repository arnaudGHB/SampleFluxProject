using CBS.UserServiceManagement.API.Helpers;
using CBS.UserServiceManagement.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

// Fichier : CBS.UserServiceManagement.API/Middlewares/JwtValidator/JwtAuthenticationConfigurationExtension.cs

using CBS.UserServiceManagement.API.Helpers;
using CBS.UserServiceManagement.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.API.Middlewares.JwtValidator
{
    public static class JwtAuthenticationConfigurationExtension
    {
        public static void AddJwtAuthenticationConfiguration(this IServiceCollection services, JwtSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings), "JwtSettings configuration is missing");
            }

            if (string.IsNullOrWhiteSpace(settings.Key))
            {
                throw new ArgumentException("JWT Key is missing in configuration. Please check appsettings.json", nameof(settings.Key));
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions =>
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
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.SecurityToken is JwtSecurityToken accessToken)
                        {
                            var userInfoToken = context.HttpContext.RequestServices.GetRequiredService<UserInfoToken>();
                            userInfoToken.Id = accessToken.Claims.FirstOrDefault(a => a.Type == "Id")?.Value;
                            userInfoToken.Email = accessToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
                            userInfoToken.Role = accessToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Role)?.Value;
                            userInfoToken.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                            
                            context.HttpContext.Items["UserId"] = userInfoToken.Id;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
        }
    }
}
