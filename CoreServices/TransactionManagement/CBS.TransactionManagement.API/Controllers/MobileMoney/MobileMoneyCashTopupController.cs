using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.API.Controllers;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.MediatR.MobileMoney.Commands;
using CBS.TransactionManagement.MediatR.MobileMoney.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.MobileMoney.Controllers
{
    // MobileMoneyCashTopup Controller to manage mobile money cash top-up requests.
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class MobileMoneyCashTopupController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for RemittanceController.
        /// Injects the IMediator service to handle requests.
        /// </summary>
        /// <param name="mediator">IMediator instance to handle queries and commands</param>
        public MobileMoneyCashTopupController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a specific Mobile Money Cash Top-up request by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the cash top-up request</param>
        /// <returns>Returns the cash top-up details in JSON or XML format</returns>
        [HttpGet("MobileMoneyCashTopup/Request/{id}", Name = "GetMobileMoneyCashTopupQuery")]
        [Produces("application/json", "application/xml", Type = typeof(MobileMoneyCashTopupDto))]
        public async Task<IActionResult> GetMobileMoneyCashTopupQuery(string id)
        {
            // Create the query to fetch the cash top-up request by its id
            var getCashReplenishmentQuery = new GetMobileMoneyCashTopupQuery { Id = id };
            // Send the query to the mediator
            var result = await _mediator.Send(getCashReplenishmentQuery);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves all pending Mobile Money Cash Top-up requests.
        /// </summary>
        /// <param name="moneyCashTopupQuery">Query object containing the filter criteria for cash top-ups</param>
        /// <returns>Returns a list of pending cash top-up requests in JSON or XML format</returns>
        [HttpPost("MobileMoneyCashTopup/Requests", Name = "GetAllMobileMoneyCashTopupQuery")]
        [Produces("application/json", "application/xml", Type = typeof(MobileMoneyCashTopupDto))]
        public async Task<IActionResult> GetAllMobileMoneyCashTopupQuery(GetAllMobileMoneyCashTopupQuery moneyCashTopupQuery)
        {
            // Send the query to the mediator to retrieve all pending cash top-up requests
            var result = await _mediator.Send(moneyCashTopupQuery);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Allows sub-tellers to initiate a cash top-up request for their till account.
        /// </summary>
        /// <param name="addCashReplenishmentCommand">Command object containing the details for the cash top-up request</param>
        /// <returns>Returns the newly created cash top-up request in JSON or XML format</returns>
        [HttpPost("MobileMoneyCashTopup/Request")]
        [Produces("application/json", "application/xml", Type = typeof(MobileMoneyCashTopupDto))]
        public async Task<IActionResult> AddMobileMoneyCashTopup(AddMobileMoneyCashTopupCommand addCashReplenishmentCommand)
        {
            // Send the command to the mediator to create a new cash top-up request
            var result = await _mediator.Send(addCashReplenishmentCommand);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Validates a sub-teller's cash top-up request.
        /// </summary>
        /// <param name="Id">The unique identifier of the cash top-up request to be validated</param>
        /// <param name="validateCashReplenishment">Command object containing the validation details</param>
        /// <returns>Returns the validated cash top-up request in JSON or XML format</returns>
        [HttpPut("MobileMoneyCashTopup/RequestValidation/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(MobileMoneyCashTopupDto))]
        public async Task<IActionResult> ValidateCashReplenishmentSubTellerCommand(string Id, ValidateMobileMoneyCashTopupCommand validateCashReplenishment)
        {
            // Set the id of the cash top-up request to be validated
            validateCashReplenishment.Id = Id;
            // Send the validation command to the mediator
            var result = await _mediator.Send(validateCashReplenishment);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes a Mobile Money Cash Top-up request by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the cash top-up request to be deleted</param>
        /// <returns>Returns the result of the deletion operation</returns>
        [HttpDelete("MobileMoneyCashTopup/Request/Delete/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]

        public async Task<IActionResult> DeleteMobileMoneyCashTopup(string id)
        {
            // Create the command to delete the cash top-up request
            var deleteCashReplenishmentCommand = new DeleteMobileMoneyCashTopupCommand { Id = id };
            // Send the command to the mediator
            var result = await _mediator.Send(deleteCashReplenishmentCommand);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

   
    }
}
