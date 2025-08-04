//using CBS.APICaller.Helper;
//using CBS.Refund.Dto;
//using CBS.Refund.MediatR.Commands;
//using CBS.Refund.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.MediatR.LoanProductMediaR.Queries;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Commands;
using CBS.NLoan.MediatR.RefundMediaR.Commands;
using CBS.NLoan.MediatR.RefundMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanProductRepaymentOrder
{
    /// <summary>
    /// LoanProductRepaymentOrder
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanProductRepaymentOrderController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanProductRepaymentOrder
        /// </summary>
        /// <param name="mediator"></param>
        public LoanProductRepaymentOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanProductRepaymentOrder By RepaymentOrderType
        /// </summary>
        /// <param name="LoanProductRepaymentOrderType"></param>
        /// <returns></returns>
        [HttpGet("LoanProductRepaymentOrder/{LoanProductRepaymentOrderType}", Name = "GetLoanProductRepaymentOrder")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductRepaymentOrderDto))]
        public async Task<IActionResult> GetLoanProductRepaymentOrder(string LoanProductRepaymentOrderType)
        {
            var getLoanProductRepaymentOrder = new GetLoanProductRepaymentOrderByRepaymentOrderTypeQuery { LoanProductRepaymentOrderType = LoanProductRepaymentOrderType };
            var result = await _mediator.Send(getLoanProductRepaymentOrder);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create  an LoanProductRepaymentOrder
        /// </summary>
        /// <param name="addLoanProductRepaymentOrder"></param>
        /// <returns></returns>
        [HttpPost("LoanProductRepaymentOrder")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddLoanPurpose(AddLoanProductRepaymentOrderCommand addLoanProductRepaymentOrder)
        {
            var result = await _mediator.Send(addLoanProductRepaymentOrder);
            return ReturnFormattedResponse(result);

        }
    }
}
