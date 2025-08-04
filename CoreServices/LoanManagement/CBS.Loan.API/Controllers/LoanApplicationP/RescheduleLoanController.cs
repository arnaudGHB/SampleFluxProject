//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.RescheduleLoanMediaR.Commands;
using CBS.NLoan.MediatR.RescheduleLoanMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanApplicationP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class RescheduleLoanController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// RescheduleLoan
        /// </summary>
        /// <param name="mediator"></param>
        public RescheduleLoanController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get RescheduleLoan By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("RescheduleLoan/{Id}", Name = "GetRescheduleLoan")]
        [Produces("application/json", "application/xml", Type = typeof(RescheduleLoanDto))]
        public async Task<IActionResult> GetRescheduleLoan(string Id)
        {
            var getRescheduleLoanQuery = new GetRescheduleLoanQuery { Id = Id };
            var result = await _mediator.Send(getRescheduleLoanQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All RescheduleLoans
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("RescheduleLoans")]
        [Produces("application/json", "application/xml", Type = typeof(List<RescheduleLoanDto>))]
        public async Task<IActionResult> GetRescheduleLoans()
        {
            var getAllRescheduleLoanQuery = new GetAllRescheduleLoanQuery { };
            var result = await _mediator.Send(getAllRescheduleLoanQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a RescheduleLoan
        /// </summary>
        /// <param name="addRescheduleLoanCommand"></param>
        /// <returns></returns>
        [HttpPost("RescheduleLoan")]
        [Produces("application/json", "application/xml", Type = typeof(RescheduleLoanDto))]
        public async Task<IActionResult> AddRescheduleLoan(AddRescheduleLoanCommand addRescheduleLoanCommand)
        {
            var result = await _mediator.Send(addRescheduleLoanCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update RescheduleLoan By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateRescheduleLoanCommand"></param>
        /// <returns></returns>
        [HttpPut("RescheduleLoan/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(RescheduleLoanDto))]
        public async Task<IActionResult> UpdateRescheduleLoan(string Id, UpdateRescheduleLoanCommand updateRescheduleLoanCommand)
        {
            updateRescheduleLoanCommand.Id = Id;
            var result = await _mediator.Send(updateRescheduleLoanCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete RescheduleLoan By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("RescheduleLoan/{Id}")]
        public async Task<IActionResult> DeleteRescheduleLoan(string Id)
        {
            var deleteRescheduleLoanCommand = new DeleteRescheduleLoanCommand { Id = Id };
            var result = await _mediator.Send(deleteRescheduleLoanCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
