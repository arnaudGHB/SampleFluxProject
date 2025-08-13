using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.CashCeilingMovement.Commands;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.CashCeilingMovement;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper.Model;
using CBS.TransactionManagement.MediatR.CashCeilingMovement.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Commands;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.CashCeilingMovement
{
    /// <summary>
    /// Handles Cash Ceiling Request operations including creation, validation, retrieval, and deletion.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] // Restrict access to authorized users only
    public class CashCeilingRequestController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CashCeilingRequestController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for sending commands and queries.</param>
        public CashCeilingRequestController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new Cash Ceiling Request.
        /// </summary>
        /// <param name="command">The command to create a new Cash Ceiling Request.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpPost("create")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCashCeilingRequest([FromBody] AddCashCeilingRequestCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Validates a Cash Ceiling Request.
        /// </summary>
        /// <param name="command">The command to validate the Cash Ceiling Request.</param>
        /// <returns>The updated Cash Ceiling Request details.</returns>
        [HttpPut("validate")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(CashCeilingRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateCashCeilingRequest([FromBody] ValidationCashCeilingRequestCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a Cash Ceiling Request by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the Cash Ceiling Request.</param>
        /// <returns>The Cash Ceiling Request details.</returns>
        [HttpGet("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(CashCeilingRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCashCeilingRequestById(string id)
        {
            var query = new GetCashCeilingRequestByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves all Cash Ceiling Requests with optional filters.
        /// </summary>
        /// <param name="branchId">Optional filter by Branch ID.</param>
        /// <param name="status">Optional filter by status.</param>
        /// <returns>List of Cash Ceiling Requests.</returns>
        [HttpGet("all")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<CashCeilingRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllCashCeilingRequests([FromQuery] string? branchId, [FromQuery] string? status, [FromQuery] string? userid, [FromQuery] string? requestType)
        {
            var query = new GetAllCashCeilingRequestsQuery { BranchId = branchId, Status = status, UserId=userid, RequestType=requestType };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Deletes a Cash Ceiling Request by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the Cash Ceiling Request to delete.</param>
        /// <returns>Boolean indicating success or failure.</returns>
        [HttpDelete("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCashCeilingRequest(string id)
        {
            var command = new DeleteCashCeilingRequestCommand { Id = id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}

