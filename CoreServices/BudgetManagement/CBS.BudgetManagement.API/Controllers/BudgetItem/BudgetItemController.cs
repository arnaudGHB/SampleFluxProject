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
    /// Controller for managing BudgetItem operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BudgetItemController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for BudgetItemController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public BudgetItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get BudgetItem by Id.
        /// </summary>
        /// <param name="id">The Id of the BudgetItem.</param>
        /// <returns>An BudgetItemDto object.</returns>
        [HttpGet("BudgetItem/{id}", Name = "GetBudgetItem")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetItemDto))]
        public async Task<IActionResult> GetBudgetItem(string id)
        {
            var query = new GetBudgetItemQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all BudgetItems.
        /// </summary>
        /// <returns>A list of BudgetItemDto objects.</returns>
        [HttpGet("BudgetItem")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetItemDto>))]
        public async Task<IActionResult> GetAllBudgetItems()
        {
            var query = new GetAllBudgetItemsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new BudgetItem.
        /// </summary>
        /// <param name="command">The command to add a new BudgetItem.</param>
        /// <returns>The created BudgetItemDto object.</returns>
        [HttpPost("BudgetItem")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetItemDto))]
        public async Task<IActionResult> AddBudgetItem(AddBudgetItemCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing BudgetItem by Id.
        /// </summary>
        /// <param name="Id">The Id of the BudgetItem to be updated.</param>
        /// <param name="command">The command containing the updated BudgetItem details.</param>
        /// <returns>The updated BudgetItemDto object.</returns>
        [HttpPut("BudgetItem/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetItemDto))]
        public async Task<IActionResult> UpdateBudgetItem(string Id, UpdateBudgetItemCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an BudgetItem by Id.
        /// </summary>
        /// <param name="Id">The Id of the BudgetItem to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("BudgetItem/{Id}")]
        public async Task<IActionResult> DeleteBudgetItem(string Id)
        {
            var command = new DeleteBudgetItemCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
