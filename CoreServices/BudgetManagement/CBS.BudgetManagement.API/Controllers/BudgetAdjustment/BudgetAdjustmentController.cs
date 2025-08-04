using CBS.BudgetManagement.API;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.MediatR.Commands;
using CBS.BudgetManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CBS.BudgetManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing BudgetAdjustment operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BudgetAdjustmentController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for BudgetAdjustmentController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public BudgetAdjustmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get BudgetAdjustment by Id.
        /// </summary>
        /// <param name="id">The Id of the BudgetAdjustment.</param>
        /// <returns>An BudgetAdjustmentDto object.</returns>
        [HttpGet("BudgetAdjustment/{id}", Name = "GetBudgetAdjustment")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetAdjustmentDto))]
        public async Task<IActionResult> GetBudgetAdjustment(string id)
        {
            var query = new GetBudgetAdjustmentQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all BudgetAdjustments.
        /// </summary>
        /// <returns>A list of BudgetAdjustmentDto objects.</returns>
        [HttpGet("BudgetAdjustment")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetAdjustmentDto>))]
        public async Task<IActionResult> GetAllBudgetAdjustments()
        {
            var query = new GetAllBudgetAdjustmentQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new BudgetAdjustment.
        /// </summary>
        /// <param name="command">The command to add a new BudgetAdjustment.</param>
        /// <returns>The created BudgetAdjustmentDto object.</returns>
        [HttpPost("BudgetAdjustment")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetAdjustmentDto))]
        public async Task<IActionResult> AddBudgetAdjustment(AddBudgetAdjustmentCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing BudgetAdjustment by Id.
        /// </summary>
        /// <param name="Id">The Id of the BudgetAdjustment to be updated.</param>
        /// <param name="command">The command containing the updated BudgetAdjustment details.</param>
        /// <returns>The updated BudgetAdjustmentDto object.</returns>
        [HttpPut("BudgetAdjustment/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetAdjustmentDto))]
        public async Task<IActionResult> UpdateBudgetAdjustment(string Id, UpdateBudgetAdjustmentCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an BudgetAdjustment by Id.
        /// </summary>
        /// <param name="Id">The Id of the BudgetAdjustment to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("BudgetAdjustment/{Id}")]
        public async Task<IActionResult> DeleteBudgetAdjustment(string Id)
        {
            var command = new DeleteBudgetAdjustmentCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
