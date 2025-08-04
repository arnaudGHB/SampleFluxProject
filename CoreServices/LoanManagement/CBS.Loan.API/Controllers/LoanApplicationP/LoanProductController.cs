//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.MediatR.LoanProductMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.LoanApplicationP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class LoanProductController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanProduct
        /// </summary>
        /// <param name="mediator"></param>
        public LoanProductController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanProduct By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanProduct/{LoanProductId}", Name = "GetLoanProduct")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductDto))]
        public async Task<IActionResult> GetLoanProduct(string LoanProductId)
        {
            var getLoanProductQuery = new GetLoanProductQuery { LoanProductId = LoanProductId };
            var result = await _mediator.Send(getLoanProductQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanProducts
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanProducts")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanProductDto>))]
        public async Task<IActionResult> GetLoanProducts()
        {
            var getAllLoanProductQuery = new GetAllLoanProductQuery { };
            var result = await _mediator.Send(getAllLoanProductQuery);
            return Ok(result);
        }
        /// <summary>
        /// Get All LoanProducts
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanProducts/Lighterversion")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanProductLightDto>))]
        public async Task<IActionResult> GetAllLoanProductLightQuery()
        {
            var getAllLoanProductQuery = new GetAllLoanProductLightQuery { };
            var result = await _mediator.Send(getAllLoanProductQuery);
            return Ok(result);
        }
        
        /// <summary>
        /// Get All LoanProducts
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanProductConfigurationAgregates")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanProductConfigurationAgregatesDto>))]
        public async Task<IActionResult> GetLoanProductConfigurationAgregatesDto()
        {
            var getAllLoanProductQuery = new GetAllLoanProductAgregatesQuery { };
            var result = await _mediator.Send(getAllLoanProductQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanProduct
        /// </summary>
        /// <param name="addLoanProductCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanProduct")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductDto))]
        public async Task<IActionResult> AddLoanProduct(AddLoanProductCommand addLoanProductCommand)
        {
            var result = await _mediator.Send(addLoanProductCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanProduct By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanProductCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanProduct/{LoanProductId}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductDto))]
        public async Task<IActionResult> UpdateLoanProduct(string LoanProductId, UpdateLoanProductCommand updateLoanProductCommand)
        {
            updateLoanProductCommand.Id = LoanProductId;
            var result = await _mediator.Send(updateLoanProductCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanProduct By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanProduct/{LoanProductId}")]
        public async Task<IActionResult> DeleteLoanProduct(string LoanProductId)
        {
            var deleteLoanProductCommand = new DeleteLoanProductCommand { LoanProductId = LoanProductId };
            var result = await _mediator.Send(deleteLoanProductCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
