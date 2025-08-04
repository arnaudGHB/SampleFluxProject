using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanAmortizationP
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanAmortizationController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanAmortization
        /// </summary>
        /// <param name="mediator"></param>
        public LoanAmortizationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanAmortization By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetLoanAmortization/{LoanAmortizationId}", Name = "GetLoanAmortization")]
        [Produces("application/json", "application/xml", Type = typeof(LoanAmortizationDto))]
        public async Task<IActionResult> GetLoanAmortization(string LoanAmortizationId)
        {
            var getLoanAmortizationQuery = new GetLoanAmortizationQuery { Id = LoanAmortizationId };
            var result = await _mediator.Send(getLoanAmortizationQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get all LoanAmortization By Loan Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Installments/GetAllLoanAmortizationByLoanIdQuery/{loanId}", Name = "GetAllLoanAmortizationByLoanIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(LoanAmortizationDto))]
        public async Task<IActionResult> GetAllLoanAmortizationByLoanIdQuery(string loanId)
        {
            var getLoanAmortizationQuery = new GetAllLoanAmortizationByLoanIdQuery { LoanId = loanId };
            var result = await _mediator.Send(getLoanAmortizationQuery);
            return ReturnFormattedResponse(result);
        }
       
        /// <summary>
        /// Simulate loan repayment installments
        /// </summary>
        /// <param name="simulatioLoanCommand"></param>
        /// <returns></returns>
        [HttpPost("Loan/Simulation")]
        [Produces("application/json", "application/xml", Type = typeof(LoanAmortizationDto))]
        public async Task<IActionResult> SimulateLoanAmortization(SimulateLoanInstallementQuery simulatioLoanCommand)
        {
            var result = await _mediator.Send(simulatioLoanCommand);
            return ReturnFormattedResponse(result);

        }
        
    }
}
