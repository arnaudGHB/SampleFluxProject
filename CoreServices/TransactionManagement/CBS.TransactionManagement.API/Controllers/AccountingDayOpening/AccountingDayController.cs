using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.MediatR.AccountingDayOpening.Commands;
using CBS.TransactionManagement.Queries.AccountingDayOpening;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.AccountingDayOpening
{
    /// <summary>
    /// Controller responsible for handling actions related to Accounting Days.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountingDayController : BaseController
    {
        // IMediator instance used to send commands and queries to the respective handlers
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountingDayController"/> class.
        /// </summary>
        /// <param name="mediator">IMediator instance for sending commands and queries.</param>
        public AccountingDayController(IMediator mediator)
        {
            _mediator = mediator;
        }
        //GetCurrentAccountingDayQuery
        /// <summary>
        /// Retrieves the Current Accounting Day for a particular branch or Centralized.
        /// </summary>
        /// <param name="branchid">Query parameters to retrieve Accounting Day.</param>
        /// <returns>Date</returns>
        /// <response code="200">Returns the current account day in [Date]</response>
        [HttpGet("AccountingDay/GetCurrentDay/{branchid}")]
        [Produces("application/json", "application/xml", Type = typeof(DateTime))]
        public async Task<IActionResult> GetCurrentDay(string branchid=null)
        {
            var getAllFeeQuery = new GetCurrentAccountingDayQuery {BrnachId= branchid };
            var result = await _mediator.Send(getAllFeeQuery);
            return Ok(result);
        }
        /// <summary>
        /// Retrieves the Current Accounting Day by id.
        /// </summary>
        /// <param name="branchid">Query parameters to retrieve Accounting Day.</param>
        /// <returns>Date</returns>
        /// <response code="200">Returns the current account day in [Date]</response>
        [HttpGet("AccountingDay/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(DateTime))]
        public async Task<IActionResult> GetAccountingDayBYIdQuery(string id)
        {
            var getAllFeeQuery = new GetAccountingDayBYIdQuery { Id = id };
            var result = await _mediator.Send(getAllFeeQuery);
            return Ok(result);
        }
        //GetAccountingDayBYIdQuery
        /// <summary>
        /// Retrieves Accounting Day details based on the provided query parameters.
        /// </summary>
        /// <param name="getAccountingDayQuery">Query containing the parameters to retrieve Accounting Days.</param>
        /// <returns>A formatted response containing the result of the query.</returns>
        [HttpPost("AccountingDay/GetAllOpenAccountingDays")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingDayDto>))]
        public async Task<IActionResult> GetAccountingDay(GetAccountingDayQuery getAccountingDayQuery)
        {
            // Sends the GetAccountingDayQuery to the mediator, which routes it to the appropriate handler
            var result = await _mediator.Send(getAccountingDayQuery);
            // Returns the result in a standardized format (e.g., JSON or XML)
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Peform actions on specific accounting day using accounting day ID. Actions include: Delete, Reopen, Close Accounting Day.
        /// </summary>
        /// <param name="accountingDayActionsCommand">Query containing the parameters to retrieve Accounting Days.</param>
        /// <returns>A formatted response containing the result of the query.</returns>
        [HttpPost("AccountingDay/AccountingDayActionsCommand")]
        [Produces("application/json", "application/xml", Type = typeof(CloseOrOpenAccountingDayResultDto))]
        public async Task<IActionResult> AccountingDayActionsCommand(AccountingDayActionsCommand accountingDayActionsCommand)
        {
            // Sends the GetAccountingDayQuery to the mediator, which routes it to the appropriate handler
            var result = await _mediator.Send(accountingDayActionsCommand);
            // Returns the result in a standardized format (e.g., JSON or XML)
            return ReturnFormattedResponse(result);
        }
        //AccountingDayActionsCommand
        /// <summary>
        /// Opens an Accounting Day, allowing transactions to be recorded for that day.
        /// </summary>
        /// <param name="openOfAccountingDayCommand">Command to open an Accounting Day.</param>
        /// <returns>A formatted response indicating the success or failure of the operation.</returns>
        [HttpPost("AccountingDay/Open")]
        [Produces("application/json", "application/xml", Type = typeof(CloseOrOpenAccountingDayResultDto))]
        public async Task<IActionResult> OpenAccountingDay(OpenOfAccountingDayCommand openOfAccountingDayCommand)
        {
            // Sends the OpenOfAccountingDayCommand to the mediator, which routes it to the appropriate handler
            var result = await _mediator.Send(openOfAccountingDayCommand);
            // Returns the result in a standardized format (e.g., JSON or XML)
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Closes an Accounting Day, preventing any further transactions from being recorded.
        /// </summary>
        /// <param name="closeOfAccountingDayCommand">Command to close an Accounting Day.</param>
        /// <returns>A formatted response indicating the success or failure of the operation.</returns>
        [HttpPost("AccountingDay/Close")]
        [Produces("application/json", "application/xml", Type = typeof(CloseOrOpenAccountingDayResultDto))]
        public async Task<IActionResult> CloseAccountingDay(CloseOfAccountingDayCommand closeOfAccountingDayCommand)
        {
            // Sends the CloseOfAccountingDayCommand to the mediator, which routes it to the appropriate handler
            var result = await _mediator.Send(closeOfAccountingDayCommand);
            // Returns the result in a standardized format (e.g., JSON or XML)
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes an Accounting Day, removing it from the system.
        /// </summary>
        /// <param name="deleteAccountingDayCommand">Command to delete an Accounting Day.</param>
        /// <returns>A formatted response indicating the success or failure of the operation.</returns>
        [HttpPost("AccountingDay/Delete")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> DeleteAccountingDay(DeleteAccountingDayCommand deleteAccountingDayCommand)
        {
            // Sends the DeleteAccountingDayCommand to the mediator, which routes it to the appropriate handler
            var result = await _mediator.Send(deleteAccountingDayCommand);
            // Returns the result in a standardized format (e.g., JSON or XML)
            return ReturnFormattedResponse(result);
        }
    }
}
