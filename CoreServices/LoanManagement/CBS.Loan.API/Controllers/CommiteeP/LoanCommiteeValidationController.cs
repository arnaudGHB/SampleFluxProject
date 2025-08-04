//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Commands;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.CommiteeP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanCommiteeValidationController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanCommiteeValidation
        /// </summary>
        /// <param name="mediator"></param>
        public LoanCommiteeValidationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanCommiteeValidation By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanCommiteeValidation/{Id}", Name = "GetLoanCommiteeValidation")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeGroupDto))]
        public async Task<IActionResult> GetLoanCommiteeValidation(string Id)
        {
            var getLoanCommiteeValidationQuery = new GetLoanCommiteeValidationHistoryQuery { Id = Id };
            var result = await _mediator.Send(getLoanCommiteeValidationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanCommiteeValidations
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanCommiteeValidations")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanCommiteeGroupDto>))]
        public async Task<IActionResult> GetLoanCommiteeValidations()
        {
            var getAllLoanCommiteeValidationQuery = new GetAllLoanCommiteeValidationHistoryQuery { };
            var result = await _mediator.Send(getAllLoanCommiteeValidationQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanCommiteeValidation
        /// </summary>
        /// <param name="addLoanCommiteeValidationCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanCommiteeValidation")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeGroupDto))]
        public async Task<IActionResult> AddLoanCommiteeValidation(AddLoanCommiteeValidationHistoryCommand addLoanCommiteeValidationCommand)
        {
            var result = await _mediator.Send(addLoanCommiteeValidationCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanCommiteeValidation By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanCommiteeValidationCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanCommiteeValidation/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeGroupDto))]
        public async Task<IActionResult> UpdateLoanCommiteeValidation(string Id, UpdateLoanCommiteeValidationHistoryCommand updateLoanCommiteeValidationCommand)
        {
            updateLoanCommiteeValidationCommand.Id = Id;
            var result = await _mediator.Send(updateLoanCommiteeValidationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanCommiteeValidation By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanCommiteeValidation/{Id}")]
        public async Task<IActionResult> DeleteLoanCommiteeValidation(string Id)
        {
            var deleteLoanCommiteeValidationCommand = new DeleteLoanCommiteeValidationHistoryCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanCommiteeValidationCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
