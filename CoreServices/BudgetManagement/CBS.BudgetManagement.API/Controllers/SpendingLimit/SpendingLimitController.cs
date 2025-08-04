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
    /// Controller for managing SpendingLimit operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class SpendingLimitController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for SpendingLimitController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public SpendingLimitController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get SpendingLimit by Id.
        /// </summary>
        /// <param name="id">The Id of the SpendingLimit.</param>
        /// <returns>An SpendingLimitDto object.</returns>
        [HttpGet("SpendingLimit/{id}", Name = "GetSpendingLimit")]
        [Produces("application/json", "application/xml", Type = typeof(SpendingLimitDto))]
        public async Task<IActionResult> GetSpendingLimit(string id)
        {
            var query = new GetSpendingLimitQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all SpendingLimits.
        /// </summary>
        /// <returns>A list of SpendingLimitDto objects.</returns>
        [HttpGet("SpendingLimit")]
        [Produces("application/json", "application/xml", Type = typeof(List<SpendingLimitDto>))]
        public async Task<IActionResult> GetAllSpendingLimits()
        {
            var query = new GetAllSpendingLimitQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new SpendingLimit.
        /// </summary>
        /// <param name="command">The command to add a new SpendingLimit.</param>
        /// <returns>The created SpendingLimitDto object.</returns>
        [HttpPost("SpendingLimit")]
        [Produces("application/json", "application/xml", Type = typeof(SpendingLimitDto))]
        public async Task<IActionResult> AddSpendingLimit(AddSpendingLimitCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing SpendingLimit by Id.
        /// </summary>
        /// <param name="Id">The Id of the SpendingLimit to be updated.</param>
        /// <param name="command">The command containing the updated SpendingLimit details.</param>
        /// <returns>The updated SpendingLimitDto object.</returns>
        [HttpPut("SpendingLimit/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(SpendingLimitDto))]
        public async Task<IActionResult> UpdateSpendingLimit(string Id, UpdateSpendingLimitCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an SpendingLimit by Id.
        /// </summary>
        /// <param name="Id">The Id of the SpendingLimit to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("SpendingLimit/{Id}")]
        public async Task<IActionResult> DeleteSpendingLimit(string Id)
        {
            var command = new DeleteSpendingLimitCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
