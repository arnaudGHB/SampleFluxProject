using CBS.FixedAssetsManagement.Data;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CBS.FixedAssetsManagement.API
{
    /// <summary>
    /// Middleware for handling JSON Web Token (JWT) authentication.
    /// The JWTMiddleware class intercepts incoming HTTP requests, validates the JWT token,
    /// and attaches the authenticated user's information to the current HttpContext for
    /// further processing by the application.
    /// </summary>
    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JWTMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null && !IsTokenExpired(token))
            {
                AttachAccountToContext(context, token);
            }

            await _next(context);
        }

        public static bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken == null)
                    return true;

                return jsonToken.ValidTo < DateTime.UtcNow;
            }
            catch (Exception)
            {
                return true;
            }
        }

        private void AttachAccountToContext(HttpContext context, string token)
        {
            try
            {
                var jwtConfig = _configuration.GetSection("JwtSettings");
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig["issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtConfig["audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["key"])),
                    ClockSkew = TimeSpan.Zero
                };

                var handler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
                context.User = principal;

                var userInfoToken = context.RequestServices.GetRequiredService<UserInfoToken>();

                userInfoToken.Id = principal.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                userInfoToken.Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                userInfoToken.BankId = principal.Claims.FirstOrDefault(c => c.Type == "BankID")?.Value;
                userInfoToken.BranchId = principal.Claims.FirstOrDefault(c => c.Type == "BranchID")?.Value;
                userInfoToken.FullName = principal.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
                userInfoToken.BranchCode = principal.Claims.FirstOrDefault(c => c.Type == "BranchCode")?.Value;
                userInfoToken.BranchName = principal.Claims.FirstOrDefault(c => c.Type == "BranchName")?.Value;
                userInfoToken.BankCode = principal.Claims.FirstOrDefault(c => c.Type == "BankCode")?.Value;
                var claimValue = principal.Claims.FirstOrDefault(c => c.Type == "IsHeadOffice")?.Value;
                if (!string.IsNullOrEmpty(claimValue) && bool.TryParse(claimValue, out var isHeadOffice))
                {
                    userInfoToken.IsHeadOffice = isHeadOffice;
                }
                userInfoToken.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                context.Items["Id"] = userInfoToken.Id;
                context.Items["Email"] = userInfoToken.Email;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                // Handle token validation errors
            }
        }
    }
}