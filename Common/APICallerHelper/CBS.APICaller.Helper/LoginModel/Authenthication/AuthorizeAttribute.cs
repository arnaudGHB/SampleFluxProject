using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CBS.APICaller.Helper.LoginModel.Authenthication
{
    /// <summary>
    /// Custom authorization attribute for restricting access to authorized users.
    /// The AuthorizeAttribute class implements the IAuthorizationFilter interface
    /// to provide a mechanism for authorization checks on controllers and actions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// Handles the authorization logic for the specified context.
        /// </summary>
        /// <param name="context">The AuthorizationFilterContext containing the request context.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                //User is not logged in, return unauthorized response
                context.Result = new JsonResult(new { message = "Unauthorized" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
        }
    }
}