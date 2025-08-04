using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.DailyTellerP.Queries;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.MediatR.ChangeManagement.Command;
using CBS.TransactionManagement.MediatR.VaultP;
using CBS.TransactionManagement.MediatR.VaultP.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.CashReplenishment
{
    /// <summary>
    /// Handles cash change management operations, including teller and vault cash changes.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize()]
    public class CashChangeController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CashChangeController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for sending commands and queries.</param>
        public CashChangeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Handles primary teller cash change operations.
        /// </summary>
        /// <param name="primaryTellerCashChangeCommand">Command object for primary teller cash change.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("primary-teller")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PrimaryTellerCashChange([FromBody] PrimaryTellerCashChangeCommand primaryTellerCashChangeCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(primaryTellerCashChangeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles sub teller cash change operations.
        /// </summary>
        /// <param name="subTellerCashChangeCommand">Command object for sub teller cash change.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("sub-teller")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubTellerCashChange([FromBody] SubTellerCashChangeCommand subTellerCashChangeCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(subTellerCashChangeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Handles vault cash change operations.
        /// </summary>
        /// <param name="vaultCashChangeCommand">Command object for vault cash change.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("vault")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VaultCashChange([FromBody] VaultCashChangeCommand vaultCashChangeCommand)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(vaultCashChangeCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a specific cash change operation by its unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the cash change operation.</param>
        /// <returns>The cash change operation details.</returns>
        [HttpGet("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(CashChangeHistoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCashChangeById(string id)
        {
            var query = new GetCashChangeByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves cash change operation history based on filters.
        /// </summary>
        /// <param name="vaultId">Filter by Vault ID (optional).</param>
        /// <param name="subTellerId">Filter by Sub Teller ID (optional).</param>
        /// <param name="primaryTellerId">Filter by Primary Teller ID (optional).</param>
        /// <param name="branchId">Filter by Branch ID (optional).</param>
        /// <param name="startDate">Filter by start date (optional).</param>
        /// <param name="endDate">Filter by end date (optional).</param>
        /// <param name="userId">Filter by User ID (optional).</param>
        /// <returns>List of cash change operation details.</returns>
        [HttpGet("history")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<CashChangeHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCashChangeHistory(
            [FromQuery] string vaultId = null,
            [FromQuery] string subTellerId = null,
            [FromQuery] string primaryTellerId = null,
            [FromQuery] string branchId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string userId = null)
        {
            var query = new GetCashChangeHistoryQuery
            {
                VaultId = vaultId,
                SubTellerId = subTellerId,
                PrimaryTellerId = primaryTellerId,
                BranchId = branchId,
                StartDate = startDate,
                EndDate = endDate,
                UserId = userId
            };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

    }
}


