 
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.MediatR.Commands;
using CBS.FixedAssetsManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using CBS.FixedAssetsManagement.MediatR.Handlers;

namespace CBS.FixedAssetsManagement.API.Controllers
{
    /// <summary>
    /// Controller for managing Depreciation Entries.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class DepreciationEntryController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for DepreciationEntryController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public DepreciationEntryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Depreciation Entry by Id.
        /// </summary>
        /// <param name="id">The Id of the Depreciation Entry.</param>
        /// <returns>A DepreciationEntryDto object.</returns>
        [HttpGet("DepreciationEntry/{id}", Name = "GetDepreciationEntry")]
        [Produces("application/json", "application/xml", Type = typeof(DepreciationEntryDto))]
        public async Task<IActionResult> GetDepreciationEntry(string id)
        {
            var query = new GetDepreciationEntryQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Depreciation Entries.
        /// </summary>
        /// <returns>A list of DepreciationEntryDto objects.</returns>
        [HttpGet("DepreciationEntry")]
        [Produces("application/json", "application/xml", Type = typeof(List<DepreciationEntryDto>))]
        public async Task<IActionResult> GetAllDepreciationEntries()
        {
            var query = new GetAllDepreciationEntriesQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }



        /// <summary>
        /// Create a new Depreciation Entry.
        /// </summary>
        /// <param name="command">The command to add a new Depreciation Entry.</param>
        /// <returns>The created DepreciationEntryDto object.</returns>
        [HttpPost("DepreciationEntry")]
        [Produces("application/json", "application/xml", Type = typeof(DepreciationEntryDto))]
        public async Task<IActionResult> AddDepreciationEntry(AddDepreciationEntryCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Depreciation Entry by Id.
        /// </summary>
        /// <param name="Id">The Id of the Depreciation Entry to be updated.</param>
        /// <param name="command">The command containing the updated Depreciation Entry details.</param>
        /// <returns>The updated DepreciationEntryDto object.</returns>
        [HttpPut("DepreciationEntry/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(DepreciationEntryDto))]
        public async Task<IActionResult> UpdateDepreciationEntry(string Id, UpdateDepreciationEntryCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete a Depreciation Entry by Id.
        /// </summary>
        /// <param name="Id">The Id of the Depreciation Entry to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("DepreciationEntry/{Id}")]
        public async Task<IActionResult> DeleteDepreciationEntry(string Id)
        {
            var command = new DeleteDepreciationEntryCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
