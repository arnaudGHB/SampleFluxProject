using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.AuthHelper
{
    public class TokenStorageService
    {
        private string _bearerToken;
        private string _fullName;

        /// <summary>
        /// Stores the token and user details.
        /// </summary>
        public void SetToken(string bearerToken, string fullName)
        {
            _bearerToken = bearerToken;
            _fullName = fullName;
        }

        /// <summary>
        /// Gets the bearer token.
        /// </summary>
        public string GetToken() => _bearerToken;

        /// <summary>
        /// Gets the full name of the authenticated user.
        /// </summary>
        public string GetFullName() => _fullName;
    }

}
