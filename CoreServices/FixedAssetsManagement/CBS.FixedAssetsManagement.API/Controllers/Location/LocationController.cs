
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
    public class LocationController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for LocationController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Depreciation Method by Id.
        /// </summary>
        /// <param name="id">The Id of the Depreciation Method.</param>
        /// <returns>A LocationDto object.</returns>
        [HttpGet("Location/{id}", Name = "GetLocation")]
        [Produces("application/json", "application/xml", Type = typeof(LocationDto))]
        public async Task<IActionResult> GetLocation(string id)
        {
            var query = new GetLocationQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Depreciation Methods.
        /// </summary>
        /// <returns>A list of LocationDto objects.</returns>
        [HttpGet("Location")]
        [Produces("application/json", "application/xml", Type = typeof(List<LocationDto>))]
        public async Task<IActionResult> GetAllLocations()
        {
            var query = new GetAllLocationsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }



        /// <summary>
        /// Create a new Depreciation Method.
        /// </summary>
        /// <param name="command">The command to add a new Depreciation Method.</param>
        /// <returns>The created LocationDto object.</returns>
        [HttpPost("Location")]
        [Produces("application/json", "application/xml", Type = typeof(LocationDto))]
        public async Task<IActionResult> AddLocation(AddLocationCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Depreciation Method by Id.
        /// </summary>
        /// <param name="Id">The Id of the Depreciation Method to be updated.</param>
        /// <param name="command">The command containing the updated Depreciation Method details.</param>
        /// <returns>The updated LocationDto object.</returns>
        [HttpPut("Location/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LocationDto))]
        public async Task<IActionResult> UpdateLocation(string Id, UpdateLocationCommand command)
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
        [HttpDelete("Location/{Id}")]
        public async Task<IActionResult> DeleteLocation(string Id)
        {
            var command = new DeleteLocationCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
