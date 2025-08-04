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
    /// Controller for managing ProjectBudget operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ProjectBudgetController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for ProjectBudgetController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public ProjectBudgetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get ProjectBudget by Id.
        /// </summary>
        /// <param name="id">The Id of the ProjectBudget.</param>
        /// <returns>An ProjectBudgetDto object.</returns>
        [HttpGet("ProjectBudget/{id}", Name = "GetProjectBudget")]
        [Produces("application/json", "application/xml", Type = typeof(ProjectBudgetDto))]
        public async Task<IActionResult> GetProjectBudget(string id)
        {
            var query = new GetProjectBudgetQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all ProjectBudgets.
        /// </summary>
        /// <returns>A list of ProjectBudgetDto objects.</returns>
        [HttpGet("ProjectBudget")]
        [Produces("application/json", "application/xml", Type = typeof(List<ProjectBudgetDto>))]
        public async Task<IActionResult> GetAllProjectBudgets()
        {
            var query = new GetAllProjectBudgetsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new ProjectBudget.
        /// </summary>
        /// <param name="command">The command to add a new ProjectBudget.</param>
        /// <returns>The created ProjectBudgetDto object.</returns>
        [HttpPost("ProjectBudget")]
        [Produces("application/json", "application/xml", Type = typeof(ProjectBudgetDto))]
        public async Task<IActionResult> AddProjectBudget(AddProjectBudgetCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing ProjectBudget by Id.
        /// </summary>
        /// <param name="Id">The Id of the ProjectBudget to be updated.</param>
        /// <param name="command">The command containing the updated ProjectBudget details.</param>
        /// <returns>The updated ProjectBudgetDto object.</returns>
        [HttpPut("ProjectBudget/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ProjectBudgetDto))]
        public async Task<IActionResult> UpdateProjectBudget(string Id, UpdateProjectBudgetCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an ProjectBudget by Id.
        /// </summary>
        /// <param name="Id">The Id of the ProjectBudget to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("ProjectBudget/{Id}")]
        public async Task<IActionResult> DeleteProjectBudget(string Id)
        {
            var command = new DeleteProjectBudgetCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
