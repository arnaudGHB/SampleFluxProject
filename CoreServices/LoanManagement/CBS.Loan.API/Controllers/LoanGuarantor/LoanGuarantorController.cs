//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanGuarantor
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanGuarantorController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanGuarantor
        /// </summary>
        /// <param name="mediator"></param>
        public LoanGuarantorController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanGuarantor By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanGuarantor/{Id}", Name = "GetLoanGuarantor")]
        [Produces("application/json", "application/xml", Type = typeof(LoanGuarantorDto))]
        public async Task<IActionResult> GetLoanGuarantor(string Id)
        {
            var getLoanGuarantorQuery = new GetLoanGuarantorQuery { Id = Id };
            var result = await _mediator.Send(getLoanGuarantorQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanGuarantors
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanGuarantors")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanGuarantorDto>))]
        public async Task<IActionResult> GetLoanGuarantors()
        {
            var getAllLoanGuarantorQuery = new GetAllLoanGuarantorQuery { };
            var result = await _mediator.Send(getAllLoanGuarantorQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanGuarantor
        /// </summary>
        /// <param name="addLoanGuarantorCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanGuarantor")]
        [Produces("application/json", "application/xml", Type = typeof(LoanGuarantorDto))]
        public async Task<IActionResult> AddLoanGuarantor(AddLoanGuarantorCommand addLoanGuarantorCommand)
        {
            var result = await _mediator.Send(addLoanGuarantorCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanGuarantor By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanGuarantorCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanGuarantor/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanGuarantorDto))]
        public async Task<IActionResult> UpdateLoanGuarantor(string Id, UpdateLoanGuarantorCommand updateLoanGuarantorCommand)
        {
            updateLoanGuarantorCommand.Id = Id;
            var result = await _mediator.Send(updateLoanGuarantorCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanGuarantor By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanGuarantor/{Id}")]
        public async Task<IActionResult> DeleteLoanGuarantor(string Id)
        {
            var deleteLoanGuarantorCommand = new DeleteLoanGuarantorCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanGuarantorCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
