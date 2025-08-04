//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.LoanP.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.PeriodP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanUploadController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Period
        /// </summary>
        /// <param name="mediator"></param>
        public LoanUploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Upload Excel File Customer File in the system
        /// </summary>
        [HttpPost("UploadLoanFile")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanUpload>))]
        public async Task<IActionResult> UploadLoanFile([FromQuery] LoanUploadCommand uploadCustomerFileCommand)
        {
            var result = await _mediator.Send(uploadCustomerFileCommand);
            return ReturnFormattedResponse(result);
        }

    }
}
