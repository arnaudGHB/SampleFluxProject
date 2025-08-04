 
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
    public class DepreciationMethodController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for DepreciationMethodController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public DepreciationMethodController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Depreciation Method by Id.
        /// </summary>
        /// <param name="id">The Id of the Depreciation Method.</param>
        /// <returns>A DepreciationMethodDto object.</returns>
        [HttpGet("DepreciationMethod/{id}", Name = "GetDepreciationMethod")]
        [Produces("application/json", "application/xml", Type = typeof(DepreciationMethodDto))]
        public async Task<IActionResult> GetDepreciationMethod(string id)
        {
            var query = new GetDepreciationMethodQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Depreciation Methods.
        /// </summary>
        /// <returns>A list of DepreciationMethodDto objects.</returns>
        [HttpGet("DepreciationMethod")]
        [Produces("application/json", "application/xml", Type = typeof(List<DepreciationMethodDto>))]
        public async Task<IActionResult> GetAllDepreciationMethods()
        {
            var query = new GetAllDepreciationMethodsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

     

        /// <summary>
        /// Create a new Depreciation Method.
        /// </summary>
        /// <param name="command">The command to add a new Depreciation Method.</param>
        /// <returns>The created DepreciationMethodDto object.</returns>
        [HttpPost("DepreciationMethod")]
        [Produces("application/json", "application/xml", Type = typeof(DepreciationMethodDto))]
        public async Task<IActionResult> AddDepreciationMethod(AddDepreciationMethodCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Depreciation Method by Id.
        /// </summary>
        /// <param name="Id">The Id of the Depreciation Method to be updated.</param>
        /// <param name="command">The command containing the updated Depreciation Method details.</param>
        /// <returns>The updated DepreciationMethodDto object.</returns>
        [HttpPut("DepreciationMethod/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(DepreciationMethodDto))]
        public async Task<IActionResult> UpdateDepreciationMethod(string Id, UpdateDepreciationMethodCommand command)
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
        [HttpDelete("DepreciationMethod/{Id}")]
        public async Task<IActionResult> DeleteDepreciationMethod(string Id)
        {
            var command = new DeleteDepreciationMethodCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
