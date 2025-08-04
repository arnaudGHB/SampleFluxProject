//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Commands;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Queries;
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
    public class LoanCommiteeValidationCriteriaController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanCommiteeValidationCriteria
        /// </summary>
        /// <param name="mediator"></param>
        public LoanCommiteeValidationCriteriaController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanCommiteeValidationCriteria By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanCommiteeValidationCriteria/{Id}", Name = "GetLoanCommiteeValidationCriteria")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeValidationHistoryDto))]
        public async Task<IActionResult> GetLoanCommiteeValidationCriteria(string Id)
        {
            var getLoanCommiteeValidationCriteriaQuery = new GetLoanCommiteeGroupQuery { Id = Id };
            var result = await _mediator.Send(getLoanCommiteeValidationCriteriaQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanCommiteeValidationCriterias
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanCommiteeValidationCriterias")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanCommiteeValidationHistoryDto>))]
        public async Task<IActionResult> GetLoanCommiteeValidationCriterias()
        {
            var getAllLoanCommiteeValidationCriteriaQuery = new GetAllLoanCommiteeGroupQuery { };
            var result = await _mediator.Send(getAllLoanCommiteeValidationCriteriaQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanCommiteeValidationCriteria
        /// </summary>
        /// <param name="addLoanCommiteeValidationCriteriaCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanCommiteeValidationCriteria")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeValidationHistoryDto))]
        public async Task<IActionResult> AddLoanCommiteeValidationCriteria(AddLoanCommiteeGroupCommand addLoanCommiteeValidationCriteriaCommand)
        {
            var result = await _mediator.Send(addLoanCommiteeValidationCriteriaCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanCommiteeValidationCriteria By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanCommiteeValidationCriteriaCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanCommiteeValidationCriteria/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeValidationHistoryDto))]
        public async Task<IActionResult> UpdateLoanCommiteeValidationCriteria(string Id, UpdateLoanCommiteeGroupCommand updateLoanCommiteeValidationCriteriaCommand)
        {
            updateLoanCommiteeValidationCriteriaCommand.Id = Id;
            var result = await _mediator.Send(updateLoanCommiteeValidationCriteriaCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanCommiteeValidationCriteria By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanCommiteeValidationCriteria/{Id}")]
        public async Task<IActionResult> DeleteLoanCommiteeValidationCriteria(string Id)
        {
            var deleteLoanCommiteeValidationCriteriaCommand = new DeleteLoanCommiteeGroupCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanCommiteeValidationCriteriaCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
