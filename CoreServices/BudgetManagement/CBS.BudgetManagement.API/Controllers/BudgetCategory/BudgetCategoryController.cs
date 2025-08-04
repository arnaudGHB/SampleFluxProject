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
    /// Controller for managing BudgetCategory operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BudgetCategoryController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for BudgetCategoryController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public BudgetCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get BudgetCategory by Id.
        /// </summary>
        /// <param name="id">The Id of the BudgetCategory.</param>
        /// <returns>An BudgetCategoryDto object.</returns>
        [HttpGet("BudgetCategory/{id}", Name = "GetBudgetCategory")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetCategoryDto))]
        public async Task<IActionResult> GetBudgetCategory(string id)
        {
            var query = new GetBudgetCategoryQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all BudgetCategorys.
        /// </summary>
        /// <returns>A list of BudgetCategoryDto objects.</returns>
        [HttpGet("BudgetCategory")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetCategoryDto>))]
        public async Task<IActionResult> GetAllBudgetCategorys()
        {
            var query = new GetAllBudgetCategoryrsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new BudgetCategory.
        /// </summary>
        /// <param name="command">The command to add a new BudgetCategory.</param>
        /// <returns>The created BudgetCategoryDto object.</returns>
        [HttpPost("BudgetCategory")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetCategoryDto))]
        public async Task<IActionResult> AddBudgetCategory(AddBudgetCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing BudgetCategory by Id.
        /// </summary>
        /// <param name="Id">The Id of the BudgetCategory to be updated.</param>
        /// <param name="command">The command containing the updated BudgetCategory details.</param>
        /// <returns>The updated BudgetCategoryDto object.</returns>
        [HttpPut("BudgetCategory/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetCategoryDto))]
        public async Task<IActionResult> UpdateBudgetCategory(string Id, UpdateBudgetCategoryCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an BudgetCategory by Id.
        /// </summary>
        /// <param name="Id">The Id of the BudgetCategory to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("BudgetCategory/{Id}")]
        public async Task<IActionResult> DeleteBudgetCategory(string Id)
        {
            var command = new DeleteBudgetCategoryCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
