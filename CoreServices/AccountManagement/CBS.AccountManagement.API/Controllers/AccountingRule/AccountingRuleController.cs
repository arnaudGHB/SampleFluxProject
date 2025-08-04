using CBS.AccountManagement.Data;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// Account
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountingRuleController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountingRule
        /// </summary>
        /// <param name="mediator"></param>
        public AccountingRuleController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get AccountingRule By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingRule/{id}", Name = "GetAccountingRule")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingRuleDto))]
        public async Task<IActionResult> GetAccountingRule(string id)
        {
            var getAccountingRuleQuery = new GetAccountingRuleQuery { Id = id };
            var result = await _mediator.Send(getAccountingRuleQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All AccountingRules
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountingRules")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingRuleDto>))]
        public async Task<IActionResult> GetAccountingRules()
        {
            var getAllAccountingRuleQuery = new GetAllAccountingRuleQuery { };
            var result = await _mediator.Send(getAllAccountingRuleQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a AccountingRule
        /// </summary>
        /// <param name="addAccountingRuleCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingRules")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingRuleDto))]
        public async Task<IActionResult> AddAccountingRule(AddAccountingRuleCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update AccountingRule By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountingRuleCommand"></param>
        /// <returns></returns>
        [HttpPut("AccountingRule/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingRuleDto))]
        public async Task<IActionResult> UpdateAccountingRule(string Id, UpdateAccountingEventRuleCommand updateAccountingRuleCommand)
        {
            updateAccountingRuleCommand.Id = Id;
            var result = await _mediator.Send(updateAccountingRuleCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete AccountingRule By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("AccountingRule/{Id}")]
        public async Task<IActionResult> DeleteAccountingRule(string Id)
        {
            var deleteAccountingRuleCommand = new DeleteAccountingRuleCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountingRuleCommand);
            return ReturnFormattedResponse(result);
        }
    }
}