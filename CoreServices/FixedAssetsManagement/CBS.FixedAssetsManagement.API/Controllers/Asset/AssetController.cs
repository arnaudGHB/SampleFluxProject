using CBS.FixedAssetsManagement.API;
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
    /// Controller for managing Asset operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AssetController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for AssetController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public AssetController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Asset by Id.
        /// </summary>
        /// <param name="id">The Id of the Asset.</param>
        /// <returns>An AssetDto object.</returns>
        [HttpGet("Asset/{id}", Name = "GetAsset")]
        [Produces("application/json", "application/xml", Type = typeof(AssetDto))]
        public async Task<IActionResult> GetAsset(string id)
        {
            var query = new GetAssetQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Assets.
        /// </summary>
        /// <returns>A list of AssetDto objects.</returns>
        [HttpGet("Asset")]
        [Produces("application/json", "application/xml", Type = typeof(List<AssetDto>))]
        public async Task<IActionResult> GetAllAssets()
        {
            var query = new GetAllAssetsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 

        /// <summary>
        /// Create a new Asset.
        /// </summary>
        /// <param name="command">The command to add a new Asset.</param>
        /// <returns>The created AssetDto object.</returns>
        [HttpPost("Asset")]
        [Produces("application/json", "application/xml", Type = typeof(AssetDto))]
        public async Task<IActionResult> AddAsset(AddAssetCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Asset by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset to be updated.</param>
        /// <param name="command">The command containing the updated Asset details.</param>
        /// <returns>The updated AssetDto object.</returns>
        [HttpPut("Asset/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AssetDto))]
        public async Task<IActionResult> UpdateAsset(string Id, UpdateAssetCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an Asset by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("Asset/{Id}")]
        public async Task<IActionResult> DeleteAsset(string Id)
        {
            var command = new DeleteAssetCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
