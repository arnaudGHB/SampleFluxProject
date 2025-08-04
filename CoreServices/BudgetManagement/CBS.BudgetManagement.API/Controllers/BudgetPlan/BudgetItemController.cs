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
    /// Controller for managing BudgetPlan operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class BudgetPlanController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for BudgetPlanController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public BudgetPlanController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get BudgetPlan by Id.
        /// </summary>
        /// <param name="id">The Id of the BudgetPlan.</param>
        /// <returns>An BudgetPlanDto object.</returns>
        [HttpGet("BudgetPlan/{id}", Name = "GetBudgetPlan")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetPlanDto))]
        public async Task<IActionResult> GetBudgetPlan(string id)
        {
            var query = new GetBudgetPlanQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all BudgetPlans.
        /// </summary>
        /// <returns>A list of BudgetPlanDto objects.</returns>
        [HttpGet("BudgetPlan")]
        [Produces("application/json", "application/xml", Type = typeof(List<BudgetPlanDto>))]
        public async Task<IActionResult> GetAllBudgetPlans()
        {
            var query = new GetAllBudgetPlanQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new BudgetPlan.
        /// </summary>
        /// <param name="command">The command to add a new BudgetPlan.</param>
        /// <returns>The created BudgetPlanDto object.</returns>
        [HttpPost("BudgetPlan")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetPlanDto))]
        public async Task<IActionResult> AddBudgetPlan(AddBudgetPlanCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing BudgetPlan by Id.
        /// </summary>
        /// <param name="Id">The Id of the BudgetPlan to be updated.</param>
        /// <param name="command">The command containing the updated BudgetPlan details.</param>
        /// <returns>The updated BudgetPlanDto object.</returns>
        [HttpPut("BudgetPlan/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(BudgetPlanDto))]
        public async Task<IActionResult> UpdateBudgetPlan(string Id, UpdateBudgetPlanCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an BudgetPlan by Id.
        /// </summary>
        /// <param name="Id">The Id of the BudgetPlan to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("BudgetPlan/{Id}")]
        public async Task<IActionResult> DeleteBudgetPlan(string Id)
        {
            var command = new DeleteBudgetPlanCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
