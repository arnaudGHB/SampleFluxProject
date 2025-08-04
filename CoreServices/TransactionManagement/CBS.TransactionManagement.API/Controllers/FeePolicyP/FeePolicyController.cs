using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.MediatR.FeeP.Queries;
using CBS.TransactionManagement.MediatR.FeePolicyP.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.TransactionManagement.API.Controllers.FeePolicyRangeP
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class FeePolicyController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// FeePolicy
        /// </summary>
        /// <param name="mediator"></param>
        public FeePolicyController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get FeePolicy By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FeePolicy/{Id}", Name = "GetFeePolicy")]
        [Produces("application/json", "application/xml", Type = typeof(FeePolicyDto))]
        public async Task<IActionResult> GetFeePolicy(string Id)
        {
            var getFeePolicyQuery = new GetFeePolicyQuery { Id = Id };
            var result = await _mediator.Send(getFeePolicyQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All FeePolicys
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("FeePolicys")]
        [Produces("application/json", "application/xml", Type = typeof(List<FeePolicyDto>))]
        public async Task<IActionResult> GetFeePolicys()
        {
            var getAllFeePolicyQuery = new GetAllFeePolicyQuery { };
            var result = await _mediator.Send(getAllFeePolicyQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a FeePolicy
        /// </summary>
        /// <param name="addFeePolicyCommand"></param>
        /// <returns></returns>
        [HttpPost("FeePolicy")]
        [Produces("application/json", "application/xml", Type = typeof(FeePolicyDto))]
        public async Task<IActionResult> AddFeePolicy(AddFeePolicyCommand addFeePolicyCommand)
        {
            var result = await _mediator.Send(addFeePolicyCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update FeePolicy By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateFeePolicyCommand"></param>
        /// <returns></returns>
        [HttpPut("FeePolicy/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(FeePolicyDto))]
        public async Task<IActionResult> UpdateFeePolicy(string Id, UpdateFeePolicyCommand updateFeePolicyCommand)
        {
            updateFeePolicyCommand.Id = Id;
            var result = await _mediator.Send(updateFeePolicyCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete FeePolicy By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("FeePolicy/{Id}")]
        public async Task<IActionResult> DeleteFeePolicy(string Id)
        {
            var deleteFeePolicyCommand = new DeleteFeePolicyCommand { Id = Id };
            var result = await _mediator.Send(deleteFeePolicyCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
