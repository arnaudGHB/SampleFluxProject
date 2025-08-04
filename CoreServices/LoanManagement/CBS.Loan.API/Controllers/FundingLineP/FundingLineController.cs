//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.MediatR.FundingLineMediaR.Commands;
using CBS.NLoan.MediatR.FundingLineMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.FundingLineP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class FundingLineController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// FundingLine
        /// </summary>
        /// <param name="mediator"></param>
        public FundingLineController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get FundingLine By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("FundingLine/{Id}", Name = "GetFundingLine")]
        [Produces("application/json", "application/xml", Type = typeof(FundingLineDto))]
        public async Task<IActionResult> GetFundingLine(string Id)
        {
            var getFundingLineQuery = new GetFundingLineQuery { Id = Id };
            var result = await _mediator.Send(getFundingLineQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All FundingLines
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("FundingLines")]
        [Produces("application/json", "application/xml", Type = typeof(List<FundingLineDto>))]
        public async Task<IActionResult> GetFundingLines()
        {
            var getAllFundingLineQuery = new GetAllFundingLineQuery { };
            var result = await _mediator.Send(getAllFundingLineQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a FundingLine
        /// </summary>
        /// <param name="addFundingLineCommand"></param>
        /// <returns></returns>
        [HttpPost("FundingLine")]
        [Produces("application/json", "application/xml", Type = typeof(FundingLineDto))]
        public async Task<IActionResult> AddFundingLine(AddFundingLineCommand addFundingLineCommand)
        {
            var result = await _mediator.Send(addFundingLineCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update FundingLine By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateFundingLineCommand"></param>
        /// <returns></returns>
        [HttpPut("FundingLine/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(FundingLineDto))]
        public async Task<IActionResult> UpdateFundingLine(string Id, UpdateFundingLineCommand updateFundingLineCommand)
        {
            updateFundingLineCommand.Id = Id;
            var result = await _mediator.Send(updateFundingLineCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete FundingLine By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("FundingLine/{Id}")]
        public async Task<IActionResult> DeleteFundingLine(string Id)
        {
            var deleteFundingLineCommand = new DeleteFundingLineCommand { Id = Id };
            var result = await _mediator.Send(deleteFundingLineCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
