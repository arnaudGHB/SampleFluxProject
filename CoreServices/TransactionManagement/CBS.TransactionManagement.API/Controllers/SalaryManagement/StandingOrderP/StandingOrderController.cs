using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.SalaryManagement.StandingOrderP
{
    /// <summary>
    /// Handles all operations related to Standing Orders, including creation, updating, retrieval, and deletion.
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize] // Restrict access to authorized users only
    public class StandingOrderController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandingOrderController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator instance for sending commands and queries.</param>
        public StandingOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Creates a new Standing Order.
        /// </summary>
        /// <param name="command">The command to create a new Standing Order.</param>
        /// <returns>StandingOrderDto containing the created standing order details.</returns>
        [HttpPost("create")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(StandingOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateStandingOrder([FromBody] AddStandingOrderCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Updates an existing Standing Order.
        /// </summary>
        /// <param name="command">The command containing updated details for the Standing Order.</param>
        /// <returns>StandingOrderDto containing the updated standing order details.</returns>
        [HttpPut("update")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(StandingOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStandingOrder([FromBody] UpdateStandingOrderCommand command)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves a Standing Order by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the Standing Order.</param>
        /// <returns>StandingOrderDto containing the details of the requested standing order.</returns>
        [HttpGet("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(StandingOrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStandingOrderById(string id)
        {
            var query = new GetStandingOrderQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Retrieves all Standing Orders filtered by Branch ID.
        /// </summary>
        /// <param name="branchId">Optional filter by Branch ID.</param>
        /// <returns>List of StandingOrderDto containing the details of all matching standing orders.</returns>
        [HttpGet("all")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<StandingOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllStandingOrders([FromQuery] string? branchId)
        {
            var query = new GetAllStandingOrdersQuery { BranchId = branchId };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves all Standing Orders for a specific member.
        /// </summary>
        /// <param name="memberId">The ID of the member.</param>
        /// <returns>List of StandingOrderDto containing the details of the member's standing orders.</returns>
        [HttpGet("member/{memberId}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(List<StandingOrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStandingOrdersByMemberId(string memberId)
        {
            var query = new GetStandingOrderByMemberIdQuery { MemberId = memberId };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Deletes a Standing Order by its unique ID.
        /// </summary>
        /// <param name="id">The ID of the Standing Order to delete.</param>
        /// <returns>Boolean indicating success or failure of the deletion.</returns>
        [HttpDelete("{id}")]
        [Produces("application/json", "application/xml")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteStandingOrder(string id)
        {
            var command = new DeleteStandingOrderCommand { Id = id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}


