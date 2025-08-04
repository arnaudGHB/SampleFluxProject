//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.MediatR.PeriodMediaR.Commands;
using CBS.NLoan.MediatR.PeriodMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.PeriodP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class PeriodController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Period
        /// </summary>
        /// <param name="mediator"></param>
        public PeriodController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Period By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Period/{Id}", Name = "GetPeriod")]
        [Produces("application/json", "application/xml", Type = typeof(PeriodDto))]
        public async Task<IActionResult> GetPeriod(string Id)
        {
            var getPeriodQuery = new GetPeriodQuery { Id = Id };
            var result = await _mediator.Send(getPeriodQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Periods
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Periods")]
        [Produces("application/json", "application/xml", Type = typeof(List<PeriodDto>))]
        public async Task<IActionResult> GetPeriods()
        {
            var getAllPeriodQuery = new GetAllPeriodQuery { };
            var result = await _mediator.Send(getAllPeriodQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Period
        /// </summary>
        /// <param name="addPeriodCommand"></param>
        /// <returns></returns>
        [HttpPost("Period")]
        [Produces("application/json", "application/xml", Type = typeof(PeriodDto))]
        public async Task<IActionResult> AddPeriod(AddPeriodCommand addPeriodCommand)
        {
            var result = await _mediator.Send(addPeriodCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update Period By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updatePeriodCommand"></param>
        /// <returns></returns>
        [HttpPut("Period/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(PeriodDto))]
        public async Task<IActionResult> UpdatePeriod(string Id, UpdatePeriodCommand updatePeriodCommand)
        {
            updatePeriodCommand.Id = Id;
            var result = await _mediator.Send(updatePeriodCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Period By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("Period/{Id}")]
        public async Task<IActionResult> DeletePeriod(string Id)
        {
            var deletePeriodCommand = new DeletePeriodCommand { Id = Id };
            var result = await _mediator.Send(deletePeriodCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
