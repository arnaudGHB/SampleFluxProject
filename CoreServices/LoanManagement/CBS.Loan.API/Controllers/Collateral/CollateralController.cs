//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.MediatR.CollateralMediaR.Commands;
using CBS.NLoan.MediatR.CollateralMediaR.Queries;
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
    public class CollateralController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Collateral
        /// </summary>
        /// <param name="mediator"></param>
        public CollateralController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Collateral By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Collateral/{Id}", Name = "GetCollateral")]
        [Produces("application/json", "application/xml", Type = typeof(CollateralDto))]
        public async Task<IActionResult> GetCollateral(string Id)
        {
            var getCollateralQuery = new GetCollateralQuery { Id = Id };
            var result = await _mediator.Send(getCollateralQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All Collaterals
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Collaterals")]
        [Produces("application/json", "application/xml", Type = typeof(List<CollateralDto>))]
        public async Task<IActionResult> GetCollaterals()
        {
            var getAllCollateralQuery = new GetAllCollateralQuery { };
            var result = await _mediator.Send(getAllCollateralQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a Collateral
        /// </summary>
        /// <param name="addCollateralCommand"></param>
        /// <returns></returns>
        [HttpPost("Collateral")]
        [Produces("application/json", "application/xml", Type = typeof(CollateralDto))]
        public async Task<IActionResult> AddCollateral(AddCollateralCommand addCollateralCommand)
        {
            var result = await _mediator.Send(addCollateralCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update Collateral By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateCollateralCommand"></param>
        /// <returns></returns>
        [HttpPut("Collateral/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CollateralDto))]
        public async Task<IActionResult> UpdateCollateral(string Id, UpdateCollateralCommand updateCollateralCommand)
        {
            updateCollateralCommand.Id = Id;
            var result = await _mediator.Send(updateCollateralCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete Collateral By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("Collateral/{Id}")]
        public async Task<IActionResult> DeleteCollateral(string Id)
        {
            var deleteCollateralCommand = new DeleteCollateralCommand { Id = Id };
            var result = await _mediator.Send(deleteCollateralCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
