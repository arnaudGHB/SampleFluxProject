//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Queries;
using CBS.NLoan.MediatR.LoanProductCollateralMediaR.Commands;
using CBS.NLoan.MediatR.LoanProductCollateralMediaR.Queries;
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
    public class LoanProductCollateralController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanProductCollateral
        /// </summary>
        /// <param name="mediator"></param>
        public LoanProductCollateralController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanProductCollateral By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanProductCollateral/{Id}", Name = "GetLoanProductCollateral")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductCollateralDto))]
        public async Task<IActionResult> GetLoanProductCollateral(string Id)
        {
            var getLoanProductCollateralQuery = new GetLoanProductCollateralQuery { Id = Id };
            var result = await _mediator.Send(getLoanProductCollateralQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanProductCollaterals
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanProductCollaterals")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanProductCollateralDto>))]
        public async Task<IActionResult> GetLoanProductCollaterals()
        {
            var getAllLoanProductCollateralQuery = new GetAllLoanProductCollateralQuery { };
            var result = await _mediator.Send(getAllLoanProductCollateralQuery);
            return Ok(result);
        }
        /// <summary>
        /// Get All LoanProductCollaterals by loan product id
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("GetAllLaonApplicationCollateralByApplicationIdQuery/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanProductCollateralDto>))]
        public async Task<IActionResult> GetAllLaonApplicationCollateralByApplicationIdQuery(string id)
        {
            var getAllLaonApplicationCollateralByApplicationIdQuery = new GetAllLaonApplicationCollateralByApplicationIdQuery {loanApplicationId=id};
            var result = await _mediator.Send(getAllLaonApplicationCollateralByApplicationIdQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanProductCollateral
        /// </summary>
        /// <param name="addLoanProductCollateralCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanProductCollateral")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductCollateralDto))]
        public async Task<IActionResult> AddLoanProductCollateral(AddLoanProductCollateralCommand addLoanProductCollateralCommand)
        {
            var result = await _mediator.Send(addLoanProductCollateralCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanProductCollateral By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanProductCollateralCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanProductCollateral/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductCollateralDto))]
        public async Task<IActionResult> UpdateLoanProductCollateral(string Id, UpdateLoanProductCollateralCommand updateLoanProductCollateralCommand)
        {
            updateLoanProductCollateralCommand.Id = Id;
            var result = await _mediator.Send(updateLoanProductCollateralCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanProductCollateral By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanProductCollateral/{Id}")]
        public async Task<IActionResult> DeleteLoanProductCollateral(string Id)
        {
            var deleteLoanProductCollateralCommand = new DeleteLoanProductCollateralCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanProductCollateralCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
