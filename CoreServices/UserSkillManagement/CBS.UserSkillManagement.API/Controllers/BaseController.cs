using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CBS.UserSkillManagement.Helper;

namespace CBS.UserSkillManagement.API
{
    /// <summary>
    /// Provides a base class for all API controllers with common functionality.
    /// This abstract controller includes standard response handling, user authentication, 
    /// and common utility methods used across all derived controllers.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(ServiceResponse<T> result)
        {
            if (result == null) return NotFound();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        protected IActionResult HandlePagedResult<T>(PaginatedResponse<T> result)
        {
            if (result == null) return NotFound();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        protected string GetCurrentUserId()
        {
            // Get the current user's ID from the claims
            var userIdClaim = User.FindFirst("uid") ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            return userIdClaim?.Value;
        }
    }
}
