using CBS.APICaller.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.BankMGT.API.Controllers
{
    /// <summary>
    /// Region
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class RegionController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Region
        /// </summary>
        /// <param name="mediator"></param>
        public RegionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Region By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Region/{id}", Name = "GetRegion")]
        [Produces("application/json", "application/xml", Type = typeof(RegionDto))]
        public async Task<IActionResult> GetRegion(string id)
        {
            var getRegionQuery = new GetRegionQuery { Id = id };
            var result = await _mediator.Send(getRegionQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Regions
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Regions")]
        [Produces("application/json", "application/xml", Type = typeof(List<RegionDto>))]
        public async Task<IActionResult> GetRegions()
        {
            var getAllRegionQuery = new GetAllRegionQuery { };
            var result = await _mediator.Send(getAllRegionQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Region
        /// </summary>
        /// <param name="addRegionCommand"></param>
        /// <returns></returns>
        [HttpPost("Region")]
        [Produces("application/json", "application/xml", Type = typeof(RegionDto))]
        public async Task<IActionResult> AddRegion(AddRegionCommand addRegionCommand)
        {
            var result = await _mediator.Send(addRegionCommand);
            return ReturnFormattedResponse(result);
            
        }
        /// <summary>
        /// Update Region By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateRegionCommand"></param>
        /// <returns></returns>
        [HttpPut("Region/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(RegionDto))]
        public async Task<IActionResult> UpdateRegion(string Id, UpdateRegionCommand updateRegionCommand)
        {
            updateRegionCommand.Id = Id;
            var result = await _mediator.Send(updateRegionCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Region By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Region/{Id}")]
        public async Task<IActionResult> DeleteRegion(string Id)
        {
            var deleteRegionCommand = new DeleteRegionCommand { Id = Id };
            var result = await _mediator.Send(deleteRegionCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
