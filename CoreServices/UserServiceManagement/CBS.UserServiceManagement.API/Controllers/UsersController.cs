using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CBS.UserServiceManagement.API.Controllers
{
    // --- CORRECTION : Ajout des attributs standards du modèle ---
    [ApiController]
    [Route("api/v1/[controller]")] // Route standardisée et versionnée
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IMediator mediator, ILogger<UsersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new user in the system. (Admin role required)
        /// </summary>
        /// <param name="command">The user creation request data.</param>
        /// <returns>The newly created user's data.</returns>
        [HttpPost("Add")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 201)] // 201 Created est plus approprié pour un POST réussi
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        [ProducesResponseType(typeof(ResponseObject), 409)]
        public async Task<IActionResult> AddUser([FromBody] AddUserCommand command)
        {
            // La validation automatique via [ApiController] rend ce bloc optionnel, mais c'est une sécurité supplémentaire
            // Validation automatique par FluentValidation
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var responseObject = new ResponseObject(
                    statusCode: 400,
                    message: "Invalid request",
                    status: "FAILED",
                    errors: errors
                );
                return BadRequest(responseObject);
            }

            _logger.LogInformation("Attempting to add a new user");
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to add user: {Message}", result.Message);
            }

            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token upon success.
        /// </summary>
        /// <param name="command">The user's login credentials.</param>
        /// <returns>A JWT token.</returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            // Vérification de la validité du modèle
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                var responseObject = new ResponseObject(
                    statusCode: 400,
                    message: "Requête invalide",
                    status: "FAILED",
                    errors: errors
                );
                return BadRequest(responseObject);
            }

            _logger.LogInformation("Login attempt for email {Email}", command.Email);
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a specific user by their unique ID. (Authenticated users only)
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The requested user's data.</returns>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> GetUserById(string id)
        {
            _logger.LogInformation("Attempting to retrieve user with ID {UserId}", id);
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a list of all users. (Admin role required)
        /// </summary>
        /// <returns>A list of all users.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Admin user retrieving all users.");
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes a user from the system (soft delete). (Admin role required)
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>A confirmation of the deletion.</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ResponseObject), 200)]
        [ProducesResponseType(typeof(ResponseObject), 400)]
        [ProducesResponseType(typeof(ResponseObject), 401)]
        [ProducesResponseType(typeof(ResponseObject), 403)]
        [ProducesResponseType(typeof(ResponseObject), 404)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            _logger.LogInformation("Attempting to delete user with ID {UserId}", id);

            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command);

            return ReturnFormattedResponse(result);
        }
    }
}