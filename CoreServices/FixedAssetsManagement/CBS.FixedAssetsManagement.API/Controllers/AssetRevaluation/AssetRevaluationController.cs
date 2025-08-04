
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
    /// Controller for managing Asset Revaluation operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AssetRevaluationController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for AssetRevaluationController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public AssetRevaluationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Asset Revaluation by Id.
        /// </summary>
        /// <param name="id">The Id of the Asset Revaluation.</param>
        /// <returns>An AssetRevaluationDto object.</returns>
        [HttpGet("AssetRevaluation/{id}", Name = "GetAssetRevaluation")]
        [Produces("application/json", "application/xml", Type = typeof(AssetRevaluationDto))]
        public async Task<IActionResult> GetAssetRevaluation(string id)
        {
            var query = new GetAssetRevaluationQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Asset Revaluations.
        /// </summary>
        /// <returns>A list of AssetRevaluationDto objects.</returns>
        [HttpGet("AssetRevaluation")]
        [Produces("application/json", "application/xml", Type = typeof(List<AssetRevaluationDto>))]
        public async Task<IActionResult> GetAllAssetRevaluations()
        {
            var query = new GetAllAssetRevaluationsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new Asset Revaluation.
        /// </summary>
        /// <param name="command">The command to add a new Asset Revaluation.</param>
        /// <returns>The created AssetRevaluationDto object.</returns>
        [HttpPost("AssetRevaluation")]
        [Produces("application/json", "application/xml", Type = typeof(AssetRevaluationDto))]
        public async Task<IActionResult> AddAssetRevaluation(AddAssetRevaluationCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Asset Revaluation by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset Revaluation to be updated.</param>
        /// <param name="command">The command containing the updated Asset Revaluation details.</param>
        /// <returns>The updated AssetRevaluationDto object.</returns>
        [HttpPut("AssetRevaluation/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AssetRevaluationDto))]
        public async Task<IActionResult> UpdateAssetRevaluation(string Id, UpdateAssetRevaluationCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an Asset Revaluation by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset Revaluation to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("AssetRevaluation/{Id}")]
        public async Task<IActionResult> DeleteAssetRevaluation(string Id)
        {
            var command = new DeleteAssetRevaluationCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
