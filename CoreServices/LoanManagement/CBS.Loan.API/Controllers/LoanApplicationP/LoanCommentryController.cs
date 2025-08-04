//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.LoanCommentryMediaR.Commands;
using CBS.NLoan.MediatR.LoanCommentryMediaR.Queries;
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
    public class LoanCommentryController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanCommentry
        /// </summary>
        /// <param name="mediator"></param>
        public LoanCommentryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanCommentry By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanCommentry/{Id}", Name = "GetLoanCommentry")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommentryDto))]
        public async Task<IActionResult> GetLoanCommentry(string Id)
        {
            var getLoanCommentryQuery = new GetLoanCommentryQuery { Id = Id };
            var result = await _mediator.Send(getLoanCommentryQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanCommentrys
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanCommentries")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanCommentryDto>))]
        public async Task<IActionResult> GetLoanCommentrys()
        {
            var getAllLoanCommentryQuery = new GetAllLoanCommentryQuery { };
            var result = await _mediator.Send(getAllLoanCommentryQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanCommentry
        /// </summary>
        /// <param name="addLoanCommentryCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanCommentry")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommentryDto))]
        public async Task<IActionResult> AddLoanCommentry(AddLoanCommentryCommand addLoanCommentryCommand)
        {
            var result = await _mediator.Send(addLoanCommentryCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanCommentry By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanCommentryCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanCommentry/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanCommentryDto))]
        public async Task<IActionResult> UpdateLoanCommentry(string Id, UpdateLoanCommentryCommand updateLoanCommentryCommand)
        {
            updateLoanCommentryCommand.Id = Id;
            var result = await _mediator.Send(updateLoanCommentryCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanCommentry By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanCommentry/{Id}")]
        public async Task<IActionResult> DeleteLoanCommentry(string Id)
        {
            var deleteLoanCommentryCommand = new DeleteLoanCommentryCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanCommentryCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
