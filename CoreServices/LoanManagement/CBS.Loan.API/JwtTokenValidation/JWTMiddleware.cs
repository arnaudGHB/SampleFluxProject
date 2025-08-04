using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.Data.Dto;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

//using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CBS.NLoan.API.JwtTokenValidation
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

                // Ensure Signing Key Exists
                var signingKey = jwtConfig["key"];
                if (string.IsNullOrEmpty(signingKey))
                {
                    Debug.WriteLine("JWT signing key is missing.");
                    return;
                }
                var ValidIssuer = jwtConfig["issuer"];
                var ValidAudience = jwtConfig["audience"];
                //var keyBytes = Encoding.UTF8.GetBytes(signingKey);
                //var key = new SymmetricSecurityKey(keyBytes);

                //var validationParameters = new TokenValidationParameters
                //{
                //    ValidateIssuer = true,
                //    ValidIssuer = jwtConfig["issuer"],
                //    ValidateAudience = true,
                //    ValidAudience = jwtConfig["audience"],
                //    ValidateLifetime = true,
                //    ValidateIssuerSigningKey = true,
                //    IssuerSigningKey = key,
                //    ClockSkew = TimeSpan.Zero
                //};

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = ValidIssuer,
                    ValidateAudience = true,
                    ValidAudience = ValidAudience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                    ClockSkew = TimeSpan.Zero
                };


                var handler = new JwtSecurityTokenHandler();

                // Debug JWT Token
                var jwtToken = handler.ReadJwtToken(token);
                Debug.WriteLine($"Token Expiration: {jwtToken.ValidTo}");
                Debug.WriteLine($"Token Header: {JsonConvert.SerializeObject(jwtToken.Header)}");

                // Check for 'kid' (Key ID) if needed
                if (jwtToken.Header.TryGetValue("kid", out var kidValue))
                {
                    Debug.WriteLine($"Token has 'kid': {kidValue}");
                }
                else
                {
                    Debug.WriteLine("Warning: Token does not contain 'kid'. Some validators might require this.");
                }

                SecurityToken validatedToken;
                var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
                context.User = principal;

                // Extract claims from JWT
                var userInfoToken = context.RequestServices.GetRequiredService<UserInfoToken>();

                userInfoToken.Id = principal.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                userInfoToken.Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                userInfoToken.BankID = principal.Claims.FirstOrDefault(c => c.Type == "BankID")?.Value;
                userInfoToken.BranchID = principal.Claims.FirstOrDefault(c => c.Type == "BranchID")?.Value;
                userInfoToken.FullName = principal.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
                userInfoToken.BranchCode = principal.Claims.FirstOrDefault(c => c.Type == "BranchCode")?.Value;
                userInfoToken.BranchName = principal.Claims.FirstOrDefault(c => c.Type == "BranchName")?.Value;
                userInfoToken.BankCode = principal.Claims.FirstOrDefault(c => c.Type == "BankCode")?.Value;

                var claimValue = principal.Claims.FirstOrDefault(c => c.Type == "IsHeadOffice")?.Value;
                if (!string.IsNullOrEmpty(claimValue) && bool.TryParse(claimValue, out var isHeadOffice))
                {
                    userInfoToken.IsHeadOffice = isHeadOffice;
                }

                // Extract Authorization header
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    userInfoToken.Token = authHeader.Substring("Bearer ".Length);
                }
                else
                {
                    Debug.WriteLine("Authorization header is missing or invalid.");
                    return;
                }

                // Store in session
                context.Session.SetString("FullName", userInfoToken.FullName);
                context.Session.SetString("Token", userInfoToken.Token);
            }
            catch (SecurityTokenExpiredException ex)
            {
                Debug.WriteLine($"Token has expired: {ex.Message}");
            }
            catch (SecurityTokenSignatureKeyNotFoundException ex)
            {
                Debug.WriteLine($"Token validation failed: {ex.Message} - Possible missing or mismatched signing key.");
            }
            catch (SecurityTokenInvalidSignatureException ex)
            {
                Debug.WriteLine($"Token signature is invalid: {ex.Message} - Ensure the correct signing key is used.");
            }
            catch (SecurityTokenValidationException ex)
            {
                Debug.WriteLine($"General token validation error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
            }
        }

        //private void AttachAccountToContext(HttpContext context, string token)
        //{
        //    try
        //    {
        //        var jwtConfig = _configuration.GetSection("JwtSettings");
        //        var validationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidIssuer = jwtConfig["issuer"],
        //            ValidateAudience = true,
        //            ValidAudience = jwtConfig["audience"],
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = false,
        //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["key"])),
        //            ClockSkew = TimeSpan.Zero
        //        };

        //        var handler = new JwtSecurityTokenHandler();
        //        SecurityToken validatedToken;
        //        var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
        //        context.User = principal;

        //        var userInfoToken = context.RequestServices.GetRequiredService<UserInfoToken>();

        //        userInfoToken.Id = principal.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        //        userInfoToken.Email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        //        userInfoToken.BankID = principal.Claims.FirstOrDefault(c => c.Type == "BankID")?.Value;
        //        userInfoToken.BranchID = principal.Claims.FirstOrDefault(c => c.Type == "BranchID")?.Value;
        //        userInfoToken.FullName = principal.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
        //        userInfoToken.BranchCode = principal.Claims.FirstOrDefault(c => c.Type == "BranchCode")?.Value;
        //        userInfoToken.BranchName = principal.Claims.FirstOrDefault(c => c.Type == "BranchName")?.Value;
        //        userInfoToken.BankCode = principal.Claims.FirstOrDefault(c => c.Type == "BankCode")?.Value;
        //        var claimValue = principal.Claims.FirstOrDefault(c => c.Type == "IsHeadOffice")?.Value;
        //        if (!string.IsNullOrEmpty(claimValue) && bool.TryParse(claimValue, out var isHeadOffice))
        //        {
        //            userInfoToken.IsHeadOffice = isHeadOffice;
        //        }
        //        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        //        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        //        {
        //            userInfoToken.Token = authHeader.Substring("Bearer ".Length);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Authorization header is missing or invalid.");
        //            return; // Early exit if the token is missing or improperly formatted.
        //        }

        //        //userInfoToken.Token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        //        context.Items["Id"] = userInfoToken.Id;
        //        context.Items["Email"] = userInfoToken.Email;
        //        // Store in session
        //        context.Session.SetString("FullName", userInfoToken.FullName);
        //        context.Session.SetString("Token", userInfoToken.Token);




        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        // Handle token validation errors
        //    }
        //}
    }
}
