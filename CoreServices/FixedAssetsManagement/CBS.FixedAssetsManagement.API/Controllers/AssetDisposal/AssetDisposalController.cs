
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
    /// Controller for managing Asset Disposal operations.
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AssetDisposalController : BaseController
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Constructor for AssetDisposalController.
        /// </summary>
        /// <param name="mediator">MediatR instance for handling commands and queries.</param>
        public AssetDisposalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Asset Disposal by Id.
        /// </summary>
        /// <param name="id">The Id of the Asset Disposal.</param>
        /// <returns>An AssetDisposalDto object.</returns>
        [HttpGet("AssetDisposal/{id}", Name = "GetAssetDisposal")]
        [Produces("application/json", "application/xml", Type = typeof(AssetDisposalDto))]
        public async Task<IActionResult> GetAssetDisposal(string id)
        {
            var query = new GetAssetDisposalQuery { Id = id };
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get all Asset Disposals.
        /// </summary>
        /// <returns>A list of AssetDisposalDto objects.</returns>
        [HttpGet("AssetDisposal")]
        [Produces("application/json", "application/xml", Type = typeof(List<AssetDisposalDto>))]
        public async Task<IActionResult> GetAllAssetDisposals()
        {
            var query = new GetAllAssetDisposalsQuery();
            var result = await _mediator.Send(query);
            return ReturnFormattedResponse(result);
        }
 

        /// <summary>
        /// Create a new Asset Disposal.
        /// </summary>
        /// <param name="command">The command to add a new Asset Disposal.</param>
        /// <returns>The created AssetDisposalDto object.</returns>
        [HttpPost("AssetDisposal")]
        [Produces("application/json", "application/xml", Type = typeof(AssetDisposalDto))]
        public async Task<IActionResult> AddAssetDisposal(AddAssetDisposalCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update an existing Asset Disposal by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset Disposal to be updated.</param>
        /// <param name="command">The command containing the updated Asset Disposal details.</param>
        /// <returns>The updated AssetDisposalDto object.</returns>
        [HttpPut("AssetDisposal/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AssetDisposalDto))]
        public async Task<IActionResult> UpdateAssetDisposal(string Id, UpdateAssetDisposalCommand command)
        {
            command.Id = Id;
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete an Asset Disposal by Id.
        /// </summary>
        /// <param name="Id">The Id of the Asset Disposal to delete.</param>
        /// <returns>An HTTP response indicating the result of the operation.</returns>
        [HttpDelete("AssetDisposal/{Id}")]
        public async Task<IActionResult> DeleteAssetDisposal(string Id)
        {
            var command = new DeleteAssetDisposalCommand { Id = Id };
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
    }
}
