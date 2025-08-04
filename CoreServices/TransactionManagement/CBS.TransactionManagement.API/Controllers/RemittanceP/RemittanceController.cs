using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.API.Controllers;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Helper.Helper.Pagging;
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.MediatR.MobileMoney.Commands;
using CBS.TransactionManagement.MediatR.MobileMoney.Queries;
using CBS.TransactionManagement.MediatR.RemittanceP.Commands;
using CBS.TransactionManagement.MediatR.RemittanceP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.RemittanceP.Controllers
{
    /// <summary>
    /// Controller for managing remittance operations.
    /// Provides endpoints for creating, retrieving, validating, and deleting remittance records.
    /// Supports operations like fetching remittance charges and retrieving remittance details based on various criteria.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class RemittanceController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for RemittanceController.
        /// Injects the IMediator service to handle requests.
        /// </summary>
        /// <param name="mediator">IMediator instance to handle queries and commands</param>
        public RemittanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a specific Mobile Money Cash Top-up request by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the cash top-up request</param>
        /// <returns>Returns the cash top-up details in JSON or XML format</returns>
        [HttpGet("Remittance/Request/{id}", Name = "GetRemittanceQuery")]
        [Produces("application/json", "application/xml", Type = typeof(RemittanceDto))]
        public async Task<IActionResult> GetRemittanceQuery(string id)
        {
            // Create the query to fetch the cash top-up request by its id
            var getCashReplenishmentQuery = new GetRemittanceQuery { Id = id };
            // Send the query to the mediator
            var result = await _mediator.Send(getCashReplenishmentQuery);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves remittance records in a DataTable format for pagination and filtering.
        /// </summary>
        /// <param name="query">Query object containing pagination and filter criteria.</param>
        /// <returns>A paginated list of remittance records in JSON or XML format.</returns>
        [HttpPost("datatable", Name = "GetRemittanceDataTable")]
        [Produces("application/json", "application/xml", Type = typeof(CustomDataTable))]
        public async Task<IActionResult> GetRemittanceDataTable([FromBody] GetAllRemittanceQuery query)
        {
            if (query == null || query.DataTableOptions == null)
                return BadRequest("Invalid request parameters.");

            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Retrieves all remittance requests based on the provided query criteria.
        /// Allows filtering by date range, approval status, source or receiving branch, and other parameters.
        /// Returns the list of remittance requests in the specified format (JSON or XML).
        /// </summary>
        /// <param name="moneyCashTopupQuery">Query object containing the filter criteria for remittance requests.</param>
        /// <returns>A list of remittance requests matching the specified criteria in JSON or XML format.</returns>
        [HttpPost("Remittance/Requests", Name = "GetAllRemittanceQuery")]
        [Produces("application/json", "application/xml", Type = typeof(RemittanceDto))]
        public async Task<IActionResult> GetAllRemittanceQuery(GetAllRemittanceQuery moneyCashTopupQuery)
        {
            // Send the query to the mediator to retrieve all remittance requests
            var result = await _mediator.Send(moneyCashTopupQuery);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Generate and send an OTP for remittance cashout.
        /// </summary>
        /// <param name="generateRemittanceOTPCommand">The command containing remittance reference and receiver's phone number.</param>
        /// <returns>Returns the generated OTP details if successful.</returns>
        [HttpPost("Remittance/GenerateRemittanceOTP")]
        [Produces("application/json")]
        public async Task<IActionResult> GenerateRemittanceOTP([FromBody] GenerateRemittanceOTPCommand generateRemittanceOTPCommand)
        {
           
            var result = await _mediator.Send(generateRemittanceOTPCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Allows users to initiate a new remittance request.
        /// Processes the request and returns the details of the created remittance in the specified format (JSON or XML).
        /// </summary>
        /// <param name="addCashReplenishmentCommand">Command object containing the details for the remittance request.</param>
        /// <returns>Returns the newly created remittance request in JSON or XML format.</returns>
        [HttpPost("Remittance/Request")]
        [Produces("application/json", "application/xml", Type = typeof(RemittanceDto))]
        public async Task<IActionResult> AddRemittance(AddRemittanceCommand addCashReplenishmentCommand)
        {
            // Send the command to the mediator to create a new remittance request
            var result = await _mediator.Send(addCashReplenishmentCommand);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Validates a remittance request.
        /// Updates the status of the specified remittance based on the provided validation details.
        /// </summary>
        /// <param name="Id">The unique identifier of the remittance request to be validated.</param>
        /// <param name="validateCashReplenishment">Command object containing the validation details for the remittance.</param>
        /// <returns>Returns the validated remittance request in JSON or XML format.</returns>
        [HttpPut("Remittance/RequestValidation/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(RemittanceDto))]
        public async Task<IActionResult> ValidateRemittance(string Id, ValidationOfRemittanceCommand validateCashReplenishment)
        {
            // Set the id of the remittance request to be validated
            validateCashReplenishment.Id = Id;
            // Send the validation command to the mediator
            var result = await _mediator.Send(validateCashReplenishment);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Deletes a remittance request by its unique identifier.
        /// Processes the deletion and returns the result of the operation.
        /// </summary>
        /// <param name="id">The unique identifier of the remittance request to be deleted.</param>
        /// <returns>Returns a boolean indicating the success or failure of the deletion operation in JSON or XML format.</returns>
        [HttpDelete("Remittance/Request/Delete/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> DeleteRemittance(string id)
        {
            // Create the command to delete the remittance request
            var deleteCashReplenishmentCommand = new DeleteRemittanceCommand { Id = id };
            // Send the command to the mediator
            var result = await _mediator.Send(deleteCashReplenishmentCommand);
            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves the remittance charge for a specified remittance type, amount, and sender account number.
        /// </summary>
        /// <param name="query">Query object containing the remittance type, amount, and sender account details.</param>
        /// <returns>Returns the calculated remittance charge in JSON or XML format.</returns>
        [HttpPost("Remittance/Charge", Name = "GetRemittanceCharge")]
        [Produces("application/json", "application/xml", Type = typeof(RemittanceChargeDto))]
        public async Task<IActionResult> GetRemittanceCharge([FromBody] GetRemittanceChargeQuery query)
        {
            // Send the query to the mediator to calculate the remittance charge
            var result = await _mediator.Send(query);

            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves remittance records based on the specified query criteria.
        /// Allows filtering by query string, query value, and approval status.
        /// </summary>
        /// <param name="query">Query object containing the filtering criteria for retrieving remittance records.</param>
        /// <returns>Returns a list of remittance records matching the criteria in JSON or XML format.</returns>
        [HttpPost("Remittance/GetAll", Name = "GetAllRemittanceWildQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<RemittanceDto>))]
        public async Task<IActionResult> GetAllRemittanceWildQuery([FromBody] GetAllRemittanceWildQuery query)
        {
            // Send the query to the mediator to retrieve remittance records
            var result = await _mediator.Send(query);

            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }


    }
}
