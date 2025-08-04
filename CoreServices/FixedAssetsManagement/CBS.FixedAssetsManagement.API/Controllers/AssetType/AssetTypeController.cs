 
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
    /// Controller for managing Asset Types.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AssetTypeController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for AssetTypeController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public AssetTypeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Asset Type by Id.
        /// </summary>
        /// <param name="id">The Id of the Asset Type.</param>
        /// <returns>An AssetTypeDto object.</returns>
        [HttpGet("AssetType/{id}", Name = "GetAssetType")]
        [Produces("application/json", "application/xml", Type = typeof(AssetTypeDto))]
        public async Task<IActionResult> GetAssetType(string id)
        {
            var query = new GetAssetTypeQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Asset Types.
        /// </summary>
        /// <returns>A list of AssetTypeDto objects.</returns>
        [HttpGet("AssetType")]
        [Produces("application/json", "application/xml", Type = typeof(List<AssetTypeDto>))]
        public async Task<IActionResult> GetAllAssetTypes()
        {
            var query = new GetAllAssetTypesQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Create a new Asset Type.
        /// </summary>
        /// <param name="command">The command to add a new Asset Type.</param>
        /// <returns>The created AssetTypeDto object.</returns>
        [HttpPost("AssetType")]
        [Produces("application/json", "application/xml", Type = typeof(AssetTypeDto))]
        public async Task<IActionResult> AddAssetType(AddAssetTypeCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Asset Type by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset Type to be updated.</param>
        /// <param name="command">The command containing the updated Asset Type details.</param>
        /// <returns>The updated AssetTypeDto object.</returns>
        [HttpPut("AssetType/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AssetTypeDto))]
        public async Task<IActionResult> UpdateAssetType(string Id, UpdateAssetTypeCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an Asset Type by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset Type to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("AssetType/{Id}")]
        public async Task<IActionResult> DeleteAssetType(string Id)
        {
            var command = new DeleteAssetTypeCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
