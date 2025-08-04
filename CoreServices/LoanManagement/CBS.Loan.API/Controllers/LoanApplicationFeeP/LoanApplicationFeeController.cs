using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Queries;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanApplicationFeeP
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanApplicationFeeController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanApplicationFee
        /// </summary>
        /// <param name="mediator"></param>
        public LoanApplicationFeeController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanApplicationFee By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanApplicationFee/{Id}", Name = "GetLoanApplicationFee")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationFeeDto))]
        public async Task<IActionResult> GetLoanApplicationFee(string Id)
        {
            var getLoanApplicationFeeQuery = new GetLoanApplicationFeeQuery { Id = Id };
            var result = await _mediator.Send(getLoanApplicationFeeQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanApplicationFees
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanApplicationFees")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanApplicationFeeDto>))]
        public async Task<IActionResult> GetLoanApplicationFees()
        {
            var getAllLoanApplicationFeeQuery = new GetAllLoanApplicationFeeQuery { };
            var result = await _mediator.Send(getAllLoanApplicationFeeQuery);
            return Ok(result);
        }
        /// <summary>
        /// Get All Pending LoanApplicationFees for a particular customer
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanApplicationFees/Pending/{customerId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanApplicationFeeDto>))]
        public async Task<IActionResult> GetPendingLoanFeeByCustomerIdQuery(string customerId)
        {
            var getAllLoanApplicationFeeQuery = new GetPendingLoanFeeByCustomerIdQuery { CustomerId= customerId };
            var result = await _mediator.Send(getAllLoanApplicationFeeQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanApplicationFee
        /// </summary>
        /// <param name="addLoanApplicationFeeCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanApplicationFee")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationFeeDto))]
        public async Task<IActionResult> AddLoanApplicationFee(AddLoanApplicationFeeCommand addLoanApplicationFeeCommand)
        {
            var result = await _mediator.Send(addLoanApplicationFeeCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Loan application fee payment
        /// </summary>
        /// <param name="feePaymentConfirmationCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanApplicationFee/Payment")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> FeePaymentConfirmationCommand(FeePaymentConfirmationCommand feePaymentConfirmationCommand)
        {
            var result = await _mediator.Send(feePaymentConfirmationCommand);
            return ReturnFormattedResponse(result);

        }
        //
        /// <summary>
        /// Update LoanApplicationFee By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanApplicationFeeCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanApplicationFee/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationFeeDto))]
        public async Task<IActionResult> UpdateLoanApplicationFee(string Id, UpdateLoanApplicationFeeCommand updateLoanApplicationFeeCommand)
        {
            updateLoanApplicationFeeCommand.Id = Id;
            var result = await _mediator.Send(updateLoanApplicationFeeCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanApplicationFee By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanApplicationFee/{Id}")]
        public async Task<IActionResult> DeleteLoanApplicationFee(string Id)
        {
            var deleteLoanApplicationFeeCommand = new DeleteLoanApplicationFeeCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanApplicationFeeCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
