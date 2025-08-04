
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.MediatR.LoanTermP.Commands;
using CBS.NLoan.MediatR.LoanTermP.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanTermP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanTermController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanTerm
        /// </summary>
        /// <param name="mediator"></param>
        public LoanTermController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanTerm By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanTerm/{Id}", Name = "GetLoanTerm")]
        [Produces("application/json", "application/xml", Type = typeof(LoanTermDto))]
        public async Task<IActionResult> GetLoanTerm(string Id)
        {
            var getLoanTermQuery = new GetLoanTermQuery { Id = Id };
            var result = await _mediator.Send(getLoanTermQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanTerms
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanTerms")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanTermDto>))]
        public async Task<IActionResult> GetLoanTerms()
        {
            var getAllLoanTermQuery = new GetAllLoanTermQuery { };
            var result = await _mediator.Send(getAllLoanTermQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanTerm
        /// </summary>
        /// <param name="addLoanTermCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanTerm")]
        [Produces("application/json", "application/xml", Type = typeof(LoanTermDto))]
        public async Task<IActionResult> AddLoanTerm(AddLoanTermCommand addLoanTermCommand)
        {
            var result = await _mediator.Send(addLoanTermCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanTerm By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanTermCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanTerm/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanTermDto))]
        public async Task<IActionResult> UpdateLoanTerm(string Id, UpdateLoanTermCommand updateLoanTermCommand)
        {
            updateLoanTermCommand.Id = Id;
            var result = await _mediator.Send(updateLoanTermCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanTerm By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanTerm/{Id}")]
        public async Task<IActionResult> DeleteLoanTerm(string Id)
        {
            var deleteLoanTermCommand = new DeleteLoanTermCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanTermCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
