//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Commands;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Queries;
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
    public class LoanCommeteeMemberController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanCommeteeMember
        /// </summary>
        /// <param name="mediator"></param>
        public LoanCommeteeMemberController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanCommeteeMember By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanCommeteeMember/{Id}", Name = "GetLoanCommeteeMember")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeMemberDto))]
        public async Task<IActionResult> GetLoanCommeteeMember(string Id)
        {
            var getLoanCommeteeMemberQuery = new GetLoanCommeteeMemberQuery { Id = Id };
            var result = await _mediator.Send(getLoanCommeteeMemberQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanCommeteeMembers
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanCommeteeMembers")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanCommiteeMemberDto>))]
        public async Task<IActionResult> GetLoanCommeteeMembers()
        {
            var getAllLoanCommeteeMemberQuery = new GetAllLoanCommeteeMemberQuery { };
            var result = await _mediator.Send(getAllLoanCommeteeMemberQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanCommeteeMember
        /// </summary>
        /// <param name="addLoanCommeteeMemberCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanCommeteeMember")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeMemberDto))]
        public async Task<IActionResult> AddLoanCommeteeMember(AddLoanCommeteeMemberCommand addLoanCommeteeMemberCommand)
        {
            var result = await _mediator.Send(addLoanCommeteeMemberCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanCommeteeMember By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanCommeteeMemberCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanCommeteeMember/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommiteeMemberDto))]
        public async Task<IActionResult> UpdateLoanCommeteeMember(string Id, UpdateLoanCommeteeMemberCommand updateLoanCommeteeMemberCommand)
        {
            updateLoanCommeteeMemberCommand.Id = Id;
            var result = await _mediator.Send(updateLoanCommeteeMemberCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanCommeteeMember By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanCommeteeMember/{Id}")]
        public async Task<IActionResult> DeleteLoanCommeteeMember(string Id)
        {
            var deleteLoanCommeteeMemberCommand = new DeleteLoanCommeteeMemberCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanCommeteeMemberCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
