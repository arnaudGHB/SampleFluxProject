using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankMGT.MediatR.Validations;
using CBS.CUSTOMER.API.Controllers.Base;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.PinValidation;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace CBS.CUSTOMER.API.Controllers.Auth
{
    /// <summary>
    /// Controller for managing customers and related operations
    /// This CustomerDashboardController manages operations related to customers and their dependencies. 
     /// The controller is secured with authorization, and MediatR is used for handling queries and commands. 
    /// Each method has appropriate summaries for clarity.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for the CustomerDashboardController
        /// </summary>
        /// <param name="mediator">Mediator for handling requests and commands</param>
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

     
       

        /// <summary>
        /// Validate customer PIN
        /// </summary>
        [HttpPost("Customer/Pin/Validate")]
        [Produces("application/json", "application/xml", Type = typeof(PinValidationResponse))]
        public async Task<IActionResult> ValidateCustomerPin(PinValidationCommand pinValidationCommand)
        {

            // Validate the command using the validator
            var validator = new PinValidationCommandValidator();
            var validationResult = validator.Validate(pinValidationCommand);

            // Check if the validation passed
            if (!validationResult.IsValid)
            {
                // Validation failed, log the errors and return a Bad Request with error messages
                return BadRequest(validationResult.Errors.Select(error => error.ErrorMessage));
            }
            var result = await _mediator.Send(pinValidationCommand);
            return ReturnFormattedResponse(result);
        }

     
    }
}


