 
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.MediatR.Commands;
using CBS.FixedAssetsManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CBS.FixedAssetsManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Depreciation Methods.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class MaintenanceLogController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for MaintenanceLogController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public MaintenanceLogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Depreciation Method by Id.
        /// </summary>
        /// <param name="id">The Id of the Depreciation Method.</param>
        /// <returns>A MaintenanceLogDto object.</returns>
        [HttpGet("MaintenanceLog/{id}", Name = "GetMaintenanceLog")]
        [Produces("application/json", "application/xml", Type = typeof(MaintenanceLogDto))]
        public async Task<IActionResult> GetMaintenanceLog(string id)
        {
            var query = new GetMaintenanceLogQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Depreciation Methods.
        /// </summary>
        /// <returns>A list of MaintenanceLogDto objects.</returns>
        [HttpGet("MaintenanceLog")]
        [Produces("application/json", "application/xml", Type = typeof(List<MaintenanceLogDto>))]
        public async Task<IActionResult> GetAllMaintenanceLogs()
        {
            var query = new GetAllMaintenanceLogsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }



        /// <summary>
        /// Create a new Depreciation Method.
        /// </summary>
        /// <param name="command">The command to add a new Depreciation Method.</param>
        /// <returns>The created MaintenanceLogDto object.</returns>
        [HttpPost("MaintenanceLog")]
        [Produces("application/json", "application/xml", Type = typeof(MaintenanceLogDto))]
        public async Task<IActionResult> AddMaintenanceLog(AddMaintenanceLogCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Depreciation Method by Id.
        /// </summary>
        /// <param name="Id">The Id of the Depreciation Method to be updated.</param>
        /// <param name="command">The command containing the updated Depreciation Method details.</param>
        /// <returns>The updated MaintenanceLogDto object.</returns>
        [HttpPut("MaintenanceLog/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(MaintenanceLogDto))]
        public async Task<IActionResult> UpdateMaintenanceLog(string Id, UpdateMaintenanceLogCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete a Depreciation Method by Id.
        /// </summary>
        /// <param name="Id">The Id of the Depreciation Method to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("MaintenanceLog/{Id}")]
        public async Task<IActionResult> DeleteMaintenanceLog(string Id)
        {
            var command = new DeleteMaintenanceLogCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
