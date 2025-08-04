
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
    /// Controller for managing Asset Transfers.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AssetTransferController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for AssetTransferController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public AssetTransferController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Asset Transfer by Id.
        /// </summary>
        /// <param name="id">The Id of the Asset Transfer.</param>
        /// <returns>An AssetTransferDto object.</returns>
        [HttpGet("AssetTransfer/{id}", Name = "GetAssetTransfer")]
        [Produces("application/json", "application/xml", Type = typeof(AssetTransferDto))]
        public async Task<IActionResult> GetAssetTransfer(string id)
        {
            var query = new GetAssetTransferQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Asset Transfers.
        /// </summary>
        /// <returns>A list of AssetTransferDto objects.</returns>
        [HttpGet("AssetTransfer")]
        [Produces("application/json", "application/xml", Type = typeof(List<AssetTransferDto>))]
        public async Task<IActionResult> GetAllAssetTransfers()
        {
            var query = new GetAllAssetTransfersQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

 
        /// <summary>
        /// Create a new Asset Transfer.
        /// </summary>
        /// <param name="command">The command to add a new Asset Transfer.</param>
        /// <returns>The created AssetTransferDto object.</returns>
        [HttpPost("AssetTransfer")]
        [Produces("application/json", "application/xml", Type = typeof(AssetTransferDto))]
        public async Task<IActionResult> AddAssetTransfer(AddAssetTransferCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Asset Transfer by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset Transfer to be updated.</param>
        /// <param name="command">The command containing the updated Asset Transfer details.</param>
        /// <returns>The updated AssetTransferDto object.</returns>
        [HttpPut("AssetTransfer/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AssetTransferDto))]
        public async Task<IActionResult> UpdateAssetTransfer(string Id, UpdateAssetTransferCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an Asset Transfer by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset Transfer to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("AssetTransfer/{Id}")]
        public async Task<IActionResult> DeleteAssetTransfer(string Id)
        {
            var command = new DeleteAssetTransferCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
