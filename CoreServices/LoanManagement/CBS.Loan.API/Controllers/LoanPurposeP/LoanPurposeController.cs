//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Commands;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanPurposeP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanPurposeController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanPurpose
        /// </summary>
        /// <param name="mediator"></param>
        public LoanPurposeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanPurpose By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanPurpose/{Id}", Name = "GetLoanPurpose")]
        [Produces("application/json", "application/xml", Type = typeof(LoanPurposeDto))]
        public async Task<IActionResult> GetLoanPurpose(string Id)
        {
            var getLoanPurposeQuery = new GetLoanPurposeQuery { Id = Id };
            var result = await _mediator.Send(getLoanPurposeQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanPurposes
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanPurposes")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanPurposeDto>))]
        public async Task<IActionResult> GetLoanPurposes()
        {
            var getAllLoanPurposeQuery = new GetAllLoanPurposeQuery { };
            var result = await _mediator.Send(getAllLoanPurposeQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanPurpose
        /// </summary>
        /// <param name="addLoanPurposeCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanPurpose")]
        [Produces("application/json", "application/xml", Type = typeof(LoanPurposeDto))]
        public async Task<IActionResult> AddLoanPurpose(AddLoanPurposeCommand addLoanPurposeCommand)
        {
            var result = await _mediator.Send(addLoanPurposeCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanPurpose By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanPurposeCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanPurpose/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanPurposeDto))]
        public async Task<IActionResult> UpdateLoanPurpose(string Id, UpdateLoanPurposeCommand updateLoanPurposeCommand)
        {
            updateLoanPurposeCommand.Id = Id;
            var result = await _mediator.Send(updateLoanPurposeCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanPurpose By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanPurpose/{Id}")]
        public async Task<IActionResult> DeleteLoanPurpose(string Id)
        {
            var deleteLoanPurposeCommand = new DeleteLoanPurposeCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanPurposeCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
