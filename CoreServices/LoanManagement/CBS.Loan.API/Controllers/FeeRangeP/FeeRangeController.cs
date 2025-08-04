using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Commands;
using CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.FeeRangeRangeP
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class FeeRangeController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// FeeRange
        /// </summary>
        /// <param name="mediator"></param>
        public FeeRangeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get FeeRange By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FeeRange/{Id}", Name = "GetFeeRange")]
        [Produces("application/json", "application/xml", Type = typeof(FeeRangeDto))]
        public async Task<IActionResult> GetFeeRange(string Id)
        {
            var getFeeRangeQuery = new GetFeeRangeQuery { Id = Id };
            var result = await _mediator.Send(getFeeRangeQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All FeeRanges
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("FeeRanges")]
        [Produces("application/json", "application/xml", Type = typeof(List<FeeRangeDto>))]
        public async Task<IActionResult> GetFeeRanges()
        {
            var getAllFeeRangeQuery = new GetAllFeeRangeQuery { };
            var result = await _mediator.Send(getAllFeeRangeQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a FeeRange
        /// </summary>
        /// <param name="addFeeRangeCommand"></param>
        /// <returns></returns>
        [HttpPost("FeeRange")]
        [Produces("application/json", "application/xml", Type = typeof(FeeRangeDto))]
        public async Task<IActionResult> AddFeeRange(AddFeeRangeCommand addFeeRangeCommand)
        {
            var result = await _mediator.Send(addFeeRangeCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update FeeRange By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateFeeRangeCommand"></param>
        /// <returns></returns>
        [HttpPut("FeeRange/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(FeeRangeDto))]
        public async Task<IActionResult> UpdateFeeRange(string Id, UpdateFeeRangeCommand updateFeeRangeCommand)
        {
            updateFeeRangeCommand.Id = Id;
            var result = await _mediator.Send(updateFeeRangeCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete FeeRange By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("FeeRange/{Id}")]
        public async Task<IActionResult> DeleteFeeRange(string Id)
        {
            var deleteFeeRangeCommand = new DeleteFeeRangeCommand { Id = Id };
            var result = await _mediator.Send(deleteFeeRangeCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
