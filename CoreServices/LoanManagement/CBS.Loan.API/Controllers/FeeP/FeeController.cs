
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Commands;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.FeeP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class FeeController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Fee
        /// </summary>
        /// <param name="mediator"></param>
        public FeeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Fee By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Fee/{Id}", Name = "GetFee")]
        [Produces("application/json", "application/xml", Type = typeof(FeeDto))]
        public async Task<IActionResult> GetFee(string Id)
        {
            var getFeeQuery = new GetFeeQuery { Id = Id };
            var result = await _mediator.Send(getFeeQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Fees
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Fees")]
        [Produces("application/json", "application/xml", Type = typeof(List<FeeDto>))]
        public async Task<IActionResult> GetFees()
        {
            var getAllFeeQuery = new GetAllFeeQuery { };
            var result = await _mediator.Send(getAllFeeQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Fee
        /// </summary>
        /// <param name="addFeeCommand"></param>
        /// <returns></returns>
        [HttpPost("Fee")]
        [Produces("application/json", "application/xml", Type = typeof(FeeDto))]
        public async Task<IActionResult> AddFee(AddFeeCommand addFeeCommand)
        {
            var result = await _mediator.Send(addFeeCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update Fee By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateFeeCommand"></param>
        /// <returns></returns>
        [HttpPut("Fee/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(FeeDto))]
        public async Task<IActionResult> UpdateFee(string Id, UpdateFeeCommand updateFeeCommand)
        {
            updateFeeCommand.Id = Id;
            var result = await _mediator.Send(updateFeeCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Fee By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("Fee/{Id}")]
        public async Task<IActionResult> DeleteFee(string Id)
        {
            var deleteFeeCommand = new DeleteFeeCommand { Id = Id };
            var result = await _mediator.Send(deleteFeeCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
