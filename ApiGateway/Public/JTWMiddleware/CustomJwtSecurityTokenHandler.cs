using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CBS.Gateway.API.JTWMiddleware.CustomerJWTValidator
{
    public class CustomJwtSecurityTokenHandler : JwtSecurityTokenHandler
    {
        private readonly TimeSpan _clockSkew;

        public CustomJwtSecurityTokenHandler(TimeSpan clockSkew)
        {
            _clockSkew = clockSkew;
        }

        public override ClaimsPrincipal ValidateToken(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            // Store the original clock skew
            TimeSpan originalClockSkew = validationParameters.ClockSkew;

            try
            {
                // Set the clock skew to the custom value
                validationParameters.ClockSkew = _clockSkew;

                // Call the base class ValidateToken method
                return base.ValidateToken(token, validationParameters, out validatedToken);
            }
            finally
            {
                // Restore the original clock skew after validation
                validationParameters.ClockSkew = _clockSkew;
            }
        }
    }

}
