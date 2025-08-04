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
    /// Controller for managing Project operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ProjectController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for ProjectController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public ProjectController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Project by Id.
        /// </summary>
        /// <param name="id">The Id of the Project.</param>
        /// <returns>An ProjectDto object.</returns>
        [HttpGet("Project/{id}", Name = "GetProject")]
        [Produces("application/json", "application/xml", Type = typeof(ProjectDto))]
        public async Task<IActionResult> GetProject(string id)
        {
            var query = new GetProjectQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Projects.
        /// </summary>
        /// <returns>A list of ProjectDto objects.</returns>
        [HttpGet("Project")]
        [Produces("application/json", "application/xml", Type = typeof(List<ProjectDto>))]
        public async Task<IActionResult> GetAllProjects()
        {
            var query = new GetAllProjectQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new Project.
        /// </summary>
        /// <param name="command">The command to add a new Project.</param>
        /// <returns>The created ProjectDto object.</returns>
        [HttpPost("Project")]
        [Produces("application/json", "application/xml", Type = typeof(ProjectDto))]
        public async Task<IActionResult> AddProject(AddProjectCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Project by Id.
        /// </summary>
        /// <param name="Id">The Id of the Project to be updated.</param>
        /// <param name="command">The command containing the updated Project details.</param>
        /// <returns>The updated ProjectDto object.</returns>
        [HttpPut("Project/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ProjectDto))]
        public async Task<IActionResult> UpdateProject(string Id, UpdateProjectCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an Project by Id.
        /// </summary>
        /// <param name="Id">The Id of the Project to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("Project/{Id}")]
        public async Task<IActionResult> DeleteProject(string Id)
        {
            var command = new DeleteProjectCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
