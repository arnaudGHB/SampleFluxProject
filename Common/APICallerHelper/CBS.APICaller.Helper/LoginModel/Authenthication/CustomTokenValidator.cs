using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CBS.APICaller.Helper.LoginModel.Authenthication
{
    public class CustomTokenValidator : ISecurityTokenValidator
    {
        private readonly TokenValidationParameters _validationParameters;

        public CustomTokenValidator(TokenValidationParameters validationParameters)
        {
            _validationParameters = validationParameters ?? throw new ArgumentNullException(nameof(validationParameters));
        }

        public bool CanValidateToken => throw new NotImplementedException();

        public int MaximumTokenSizeInBytes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CanReadToken(string securityToken)
        {
            return true;
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            var handler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = null;

            try
            {
                var jwt = handler.ReadToken(securityToken) as JwtSecurityToken;

                // Validate token properties and customize claims as needed
                // For example, modify claims, validate additional properties, etc.
                // ...

                // Create a custom ClaimsIdentity and set custom claims
                var customClaimsIdentity = new ClaimsIdentity(jwt.Claims, "custom");
                // Add additional custom claims if needed
                //customClaimsIdentity.AddClaim(new Claim("CustomClaim", "CustomValue"));

                // Create a custom ClaimsPrincipal with the custom ClaimsIdentity
                principal = new ClaimsPrincipal(customClaimsIdentity);

                // Perform additional validation if necessary
                // ...

                validatedToken = jwt;
            }
            catch (Exception)
            {
                // Handle token validation errors
                validatedToken = null;
            }

            return principal;
        }
    }
}