
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.API

{
    public static class JwtAuthenticationConfigurationExtension
    {
        public static void AddJwtAutheticationConfiguration(
            this IServiceCollection services,
            JwtSettings settings)
        {
            // Register Jwt as the Authentication service
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            })
            .AddJwtBearer("Bearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters =
              new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(settings.Key)),
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
                            var userName = accessToken.Claims.FirstOrDefault(a => a.Type == Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub)?.Value;
                            var email = accessToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
                            context.HttpContext.Items["Id"] = userName;
                            if (email == null)
                            {
                                email = userName;
                            }
                            context.HttpContext.Items["Email"] = email;
                            var userInfoToken = context.HttpContext.RequestServices.GetRequiredService<UserInfoToken>();
                            userInfoToken.Id = userName;
                            userInfoToken.Email = email;

                            userInfoToken.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                            context.HttpContext.Items["Token"] = userInfoToken.Token;

                        }
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();
        }
    }
}