//using CBS.APICaller.Helper;
//using CBS.Loan.Dto;
//using CBS.Loan.MediatR.Commands;
//using CBS.Loan.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.NLoan.API.Controllers.Base;
using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Commands;
using CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.NLoan.API.Controllers.CustomerLoanAccountP
{
    /// <summary>
    /// Loan
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class CustomerLoanAccountController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// CustomerLoanAccount
        /// </summary>
        /// <param name="mediator"></param>
        public CustomerLoanAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get CustomerLoanAccount By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("CustomerLoanAccount/{Id}", Name = "GetCustomerLoanAccount")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerLoanAccountDto))]
        public async Task<IActionResult> GetCustomerLoanAccount(string Id)
        {
            var getCustomerLoanAccountQuery = new GetCustomerLoanAccountQuery { Id = Id };
            var result = await _mediator.Send(getCustomerLoanAccountQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get All CustomerLoanAccounts
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("CustomerLoanAccounts")]
        [Produces("application/json", "application/xml", Type = typeof(List<CustomerLoanAccountDto>))]
        public async Task<IActionResult> GetCustomerLoanAccounts()
        {
            var getAllCustomerLoanAccountQuery = new GetAllCustomerLoanAccountQuery { };
            var result = await _mediator.Send(getAllCustomerLoanAccountQuery);
            return Ok(result);
        }
        /// <summary>
        /// Create a CustomerLoanAccount
        /// </summary>
        /// <param name="addCustomerLoanAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("CustomerLoanAccount")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerLoanAccountDto))]
        public async Task<IActionResult> AddCustomerLoanAccount(AddCustomerLoanAccountCommand addCustomerLoanAccountCommand)
        {
            var result = await _mediator.Send(addCustomerLoanAccountCommand);
            return ReturnFormattedResponse(result);

        }
        /// <summary>
        /// Update CustomerLoanAccount By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="updateCustomerLoanAccountCommand"></param>
        /// <returns></returns>
        [HttpPut("CustomerLoanAccount/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerLoanAccountDto))]
        public async Task<IActionResult> UpdateCustomerLoanAccount(string Id, UpdateCustomerLoanAccountCommand updateCustomerLoanAccountCommand)
        {
            updateCustomerLoanAccountCommand.Id = Id;
            var result = await _mediator.Send(updateCustomerLoanAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Delete CustomerLoanAccount By Id
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [HttpDelete("CustomerLoanAccount/{Id}")]
        public async Task<IActionResult> DeleteCustomerLoanAccount(string Id)
        {
            var deleteCustomerLoanAccountCommand = new DeleteCustomerLoanAccountCommand { Id = Id };
            var result = await _mediator.Send(deleteCustomerLoanAccountCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
