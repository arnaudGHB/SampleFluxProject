//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Commands;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.Collateral
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanCollateralController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanCollateral
        /// </summary>
        /// <param name="mediator"></param>
        public LoanCollateralController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanCollateral By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanCollateral/{Id}", Name = "GetLoanCollateral")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationCollateralDto))]
        public async Task<IActionResult> GetLoanCollateral(string Id)
        {
            var getLoanCollateralQuery = new GetLoanCollateralQuery { Id = Id };
            var result = await _mediator.Send(getLoanCollateralQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanCollaterals
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanCollaterals")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanApplicationCollateralDto>))]
        public async Task<IActionResult> GetLoanCollaterals()
        {
            var getAllLoanCollateralQuery = new GetAllLoanCollateralQuery { };
            var result = await _mediator.Send(getAllLoanCollateralQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanCollateral
        /// </summary>
        /// <param name="addLoanCollateralCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanCollateral")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationCollateralDto))]
        public async Task<IActionResult> AddLoanCollateral(AddLoanCollateralCommand addLoanCollateralCommand)
        {
            var result = await _mediator.Send(addLoanCollateralCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanCollateral By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanCollateralCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanCollateral/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanApplicationCollateralDto))]
        public async Task<IActionResult> UpdateLoanCollateral(string Id, UpdateLoanCollateralCommand updateLoanCollateralCommand)
        {
            updateLoanCollateralCommand.Id = Id;
            var result = await _mediator.Send(updateLoanCollateralCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanCollateral By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanCollateral/{Id}")]
        public async Task<IActionResult> DeleteLoanCollateral(string Id)
        {
            var deleteLoanCollateralCommand = new DeleteLoanCollateralCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanCollateralCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
