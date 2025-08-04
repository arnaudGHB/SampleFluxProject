using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using CBS.APICaller.Helper;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using CBS.BankMGT.Data.Dto;
using System.Security.Claims;
using CBS.APICaller.Helper.LoginModel.Authenthication;

namespace CBS.BankMGT.API
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
                    {//((ClaimsIdentity)principal.Identity).FindFirst(ClaimTypes.Email);
                        if (context.SecurityToken is JwtSecurityToken accessToken)
                        {
                            var id = accessToken.Claims.FirstOrDefault(a => a.Type == Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub)?.Value;
                            var email = accessToken.Claims.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
                            context.HttpContext.Items["Id"] = id;
                            if (email==null)
                            {
                                email = id;
                            }
                            context.HttpContext.Items["Email"] = email;
                            var userInfoToken = context.HttpContext.RequestServices.GetRequiredService<UserInfoToken>();
                            userInfoToken.Id = id;
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
