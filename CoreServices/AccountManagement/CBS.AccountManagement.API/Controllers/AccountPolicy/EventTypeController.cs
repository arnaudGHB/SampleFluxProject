using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// AccountPolicy
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountPolicyController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountPolicy
        /// </summary>
        /// <param name="mediator"></param>
        public AccountPolicyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get AccountPolicy By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountPolicy/{id}", Name = "GetAccountPolicy")]
        [Produces("application/json", "application/xml", Type = typeof(CashMovementTrackerDto))]
        public async Task<IActionResult> GetAccountPolicy(string id)
        {
            var getAccountPolicyQuery = new GetAccountPolicyQuery { Id = id };
            var result = await _mediator.Send(getAccountPolicyQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All AccountPolicy
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountPolicies")]
        [Produces("application/json", "application/xml", Type = typeof(List<CashMovementTrackerDto>))]
        public async Task<IActionResult> GetAccountPolicy()
        {
            var getAllAccountPolicyQuery = new GetAllAccountPolicyQuery { };
            var result = await _mediator.Send(getAllAccountPolicyQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Create a AccountPolicy
        /// </summary>
        /// <param name="addAccountPolicyCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountPolicy")]
        [Produces("application/json", "application/xml", Type = typeof(CashMovementTrackerDto))]
        public async Task<IActionResult> AddAccountPolicy(AddAccountPolicyCommand addAccountPolicyCommand)
        {
            var result = await _mediator.Send(addAccountPolicyCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update AccountPolicy By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountPolicyCommand"></param>
        /// <returns></returns>
        [HttpPut("AccountPolicy/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(CashMovementTrackerDto))]
        public async Task<IActionResult> UpdateAccountPolicy(string Id, UpdateAccountPolicyCommand updateAccountPolicyCommand)
        {
            updateAccountPolicyCommand.Id = Id;
            var result = await _mediator.Send(updateAccountPolicyCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete AccountPolicy By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("AccountPolicy/{Id}")]
        public async Task<IActionResult> DeleteAccountPolicy(string Id)
        {
            var deleteAccountPolicyCommand = new DeleteAccountPolicyCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountPolicyCommand);
            return ReturnFormattedResponse(result);
        }
    }
}