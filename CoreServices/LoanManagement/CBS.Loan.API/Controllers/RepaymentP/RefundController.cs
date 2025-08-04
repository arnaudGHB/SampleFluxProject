//using CBS.APICaller.Helper;
//using CBS.Refund.Dto;
//using CBS.Refund.MediatR.Commands;
//using CBS.Refund.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.MediatR.RefundMediaR.Commands;
using CBS.NLoan.MediatR.RefundMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.RefundP
{
    /// <summary>
    /// Refund
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class RefundController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Refund
        /// </summary>
        /// <param name="mediator"></param>
        public RefundController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Refund By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Refund/{RefundId}", Name = "GetRefund")]
        [Produces("application/json", "application/xml", Type = typeof(RefundDto))]
        public async Task<IActionResult> GetRefund(string RefundId)
        {
            var getRefundQuery = new GetRefundQuery { Id = RefundId };
            var result = await _mediator.Send(getRefundQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Refunds
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Refunds")]
        [Produces("application/json", "application/xml", Type = typeof(List<RefundDto>))]
        public async Task<IActionResult> GetRefunds()
        {
            var getAllRefundQuery = new GetAllRefundQuery { };
            var result = await _mediator.Send(getAllRefundQuery);
            return Ok(result);
        }
        //
        /// <summary>
        /// Create a Refund
        /// </summary>
        /// <param name="addLoanRepaymentCommand"></param>
        /// <returns></returns>
        [HttpPost("Refund/LomsumPayment")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddLoanRepaymentCommand(AddLoanRepaymentCommand addLoanRepaymentCommand)
        {
            var result = await _mediator.Send(addLoanRepaymentCommand);
            return ReturnFormattedResponse(result);
        }  
        
        
        //
        /// <summary>
        /// Standard Order Repayment.
        /// </summary>
        /// <param name="addLoanRepaymentCommand"></param>
        /// <returns></returns>
        [HttpPost("Refund/LoanSORepayment")]
        [Produces("application/json", "application/xml", Type = typeof(SavingAmtWithRefundDto))]
        public async Task<IActionResult> AddLoanRepaymentCommand(AddLoanSORepaymentCommand addLoanRepaymentCommand)
        {
            var result = await _mediator.Send(addLoanRepaymentCommand);
            return ReturnFormattedResponse(result);
        }


        //
        /// <summary>
        /// Bulk Standard Order Repayment with SalaryCode.
        /// </summary>
        /// <param name="addLoanSOBulkRepaymentCommand"></param>
        /// <returns></returns>
        [HttpPost("Refund/LoanSOBulkRepayment")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddLoanSOBulkRepaymentCommand(AddLoanSOBulkRepaymentCommand addLoanSOBulkRepaymentCommand)
        {
            var result = await _mediator.Send(addLoanSOBulkRepaymentCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a Refund
        /// </summary>
        /// <param name="addRefundCommand"></param>
        /// <returns></returns>
        [HttpPost("Refund")]
        [Produces("application/json", "application/xml", Type = typeof(RefundDto))]
        public async Task<IActionResult> AddRefund(AddRefundCommand addRefundCommand)
        {
            var result = await _mediator.Send(addRefundCommand);
            return ReturnFormattedResponse(result);
        }

       

        //AddLoanRepaymentCommand

        /// <summary>
        /// Update Refund By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateRefundCommand"></param>
        /// <returns></returns>
        [HttpPut("Refund/{RefundId}")]
        [Produces("application/json", "application/xml", Type = typeof(RefundDto))]
        public async Task<IActionResult> UpdateRefund(string RefundId, UpdateRefundCommand updateRefundCommand)
        {
            updateRefundCommand.Id = RefundId;
            var result = await _mediator.Send(updateRefundCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Refund By Id
        /// </summary>
        /// <param name="RefundId"></param>
        /// <returns></returns>
        [HttpDelete("Refund/{RefundId}")]
        public async Task<IActionResult> DeleteRefund(string RefundId)
        {
            var deleteRefundCommand = new DeleteRefundCommand { Id = RefundId };
            var result = await _mediator.Send(deleteRefundCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
