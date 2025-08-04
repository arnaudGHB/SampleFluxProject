using Hangfire.Dashboard;

namespace CBS.TransactionManagement.API.JWTTokenValidator
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            // Ensure the user is authenticated
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            // Check if the user is in the "Administrator" role
            return httpContext.User.IsInRole("Administrator");
        }
    }
}
