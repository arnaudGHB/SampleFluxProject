
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayP.Queries;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.TransactionTrackerAccountingData
{
    /// <summary>
    /// TransactionTrackerAccounting API Controller
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class TransactionTrackerAccountingController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for TransactionTrackerAccountingController.
        /// </summary>
        /// <param name="mediator">Mediator instance for handling requests.</param>
        public TransactionTrackerAccountingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves paginated TransactionTrackerAccounting data.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="filters">Optional filters as key-value pairs.</param>
        /// <returns>A paginated list of TransactionTrackerAccounting records.</returns>
        [HttpGet("TransactionTrackerAccounting/paginate")]
        [Produces("application/json")]
        public async Task<IActionResult> GetPaginatedData(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Dictionary<string, string> filters = null)
        {
            try
            {
                // Convert filters to a usable dictionary for MongoDB
                var fieldFilters = filters?.ToDictionary(
                    filter => filter.Key,
                    filter => (object)filter.Value);

                // Create the query object
                var query = new GetTransactionTrackerAccountingPaginationQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Filters = fieldFilters
                };

                // Send query to the handler
                var result = await _mediator.Send(query);

                // Return formatted response
                return ReturnFormattedResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves a specific TransactionTrackerAccounting record by ID.
        /// </summary>
        /// <param name="id">The ID of the record to retrieve.</param>
        /// <returns>The TransactionTrackerAccounting record.</returns>
        [HttpGet("TransactionTrackerAccounting/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTransactionTrackerAccounting(string id)
        {
            var query = new GetTransactionTrackerAccountingByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Adds a new TransactionTrackerAccounting record.
        /// </summary>
        /// <param name="command">The command containing details of the new record.</param>
        /// <returns>The created record.</returns>
        [HttpPost("TransactionTrackerAccounting")]
        [Produces("application/json")]
        public async Task<IActionResult> AddTransactionTrackerAccounting([FromBody] AddTransactionTrackerAccountingCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Updates an existing TransactionTrackerAccounting record.
        /// </summary>
        /// <param name="id">The ID of the record to update.</param>
        /// <param name="command">The command containing updated details.</param>
        /// <returns>The updated record.</returns>
        [HttpPut("TransactionTrackerAccounting/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateTransactionTrackerAccounting(string id, [FromBody] UpdateTransactionTrackerAccountingCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes a specific TransactionTrackerAccounting record by ID.
        /// </summary>
        /// <param name="id">The ID of the record to delete.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        [HttpDelete("TransactionTrackerAccounting/{id}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteTransactionTrackerAccounting(string id)
        {
            var command = new DeleteTransactionTrackerAccountingCommand { Id = id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves TransactionTrackerAccounting records between specified dates.
        /// </summary>
        /// <param name="startDate">The start date for the range.</param>
        /// <param name="endDate">The end date for the range.</param>
        /// <returns>A list of TransactionTrackerAccounting records between the specified dates.</returns>
        [HttpGet("TransactionTrackerAccounting/dates")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTransactionTrackerAccountingBetweenDates([FromQuery] DateTime startDate,[FromQuery] DateTime endDate)
        {
            try
            {
                var query = new GetTransactionTrackerAccountingByDateRangeQuery
                {
                    StartDate = startDate,
                    EndDate = endDate
                };

                var result = await _mediator.Send(query);
                return ReturnFormattedResponse(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Error = ex.Message });
            }
        }
        /// <summary>
        /// Retrieves the last X transactions.
        /// </summary>
        /// <param name="transactionCount">The number of transactions to retrieve.</param>
        /// <returns>A list of the last X transactions.</returns>
        [HttpGet("TransactionTrackerAccounting/last/{transactionCount}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetLastXTransactions(int transactionCount)
        {
            var query = new GetLastXTransactionsQuery { TransactionCount = transactionCount };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves a transaction by its reference.
        /// </summary>
        /// <param name="transactionReference">The transaction reference to retrieve.</param>
        /// <returns>The transaction matching the specified reference.</returns>
        [HttpGet("TransactionTrackerAccounting/reference/{transactionReference}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetTransactionByReference(string transactionReference)
        {
            var query = new GetTransactionByReferenceQuery { TransactionReference = transactionReference };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
    }

}