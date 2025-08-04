//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.MediatR.CycleNameMediaR.Commands;
using CBS.NLoan.MediatR.CycleNameMediaR.Queries;
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
    public class LoanProductCategoryController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// LoanProductCategory
        /// </summary>
        /// <param name="mediator"></param>
        public LoanProductCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get LoanProductCategory By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("LoanProductCategory/{Id}", Name = "GetLoanProductCategory")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductCategoryDto))]
        public async Task<IActionResult> GetLoanProductCategory(string Id)
        {
            var getLoanProductCategoryQuery = new GetLoanProductCategoryQuery { Id = Id };
            var result = await _mediator.Send(getLoanProductCategoryQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All LoanProductCategorys
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("LoanProductCategorys")]
        [Produces("application/json", "application/xml", Type = typeof(List<LoanProductCategoryDto>))]
        public async Task<IActionResult> GetLoanProductCategorys()
        {
            var getAllLoanProductCategoryQuery = new GetAllLoanProductCategoryQuery { };
            var result = await _mediator.Send(getAllLoanProductCategoryQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a LoanProductCategory
        /// </summary>
        /// <param name="addLoanProductCategoryCommand"></param>
        /// <returns></returns>
        [HttpPost("LoanProductCategory")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductCategoryDto))]
        public async Task<IActionResult> AddLoanProductCategory(AddLoanProductCategoryCommand addLoanProductCategoryCommand)
        {
            var result = await _mediator.Send(addLoanProductCategoryCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update LoanProductCategory By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateLoanProductCategoryCommand"></param>
        /// <returns></returns>
        [HttpPut("LoanProductCategory/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(LoanProductCategoryDto))]
        public async Task<IActionResult> UpdateLoanProductCategory(string Id, UpdateLoanProductCategoryCommand updateLoanProductCategoryCommand)
        {
            updateLoanProductCategoryCommand.Id = Id;
            var result = await _mediator.Send(updateLoanProductCategoryCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete LoanProductCategory By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("LoanProductCategory/{Id}")]
        public async Task<IActionResult> DeleteLoanProductCategory(string Id)
        {
            var deleteLoanProductCategoryCommand = new DeleteLoanProductCategoryCommand { Id = Id };
            var result = await _mediator.Send(deleteLoanProductCategoryCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
