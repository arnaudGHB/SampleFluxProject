using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.AccountingRuleEntryDto;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// Account  GetProductConfigurationStatusQuery
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountingRuleEntryController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// AccountingRuleEntry
        /// </summary>
        /// <param name="mediator"></param>
        public AccountingRuleEntryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get AccountingRuleEntry By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingRuleEntry/{id}", Name = "GetAccountingRuleEntry")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingRuleEntryDto))]
        public async Task<IActionResult> GetAccountingRuleEntry(string id)
        {
            var getAccountingRuleEntryQuery = new GetAccountingRuleEntryQuery { Id = id };
            var result = await _mediator.Send(getAccountingRuleEntryQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get AccountingRuleEntry By EventCode
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingRuleEntry/GetAccountingRuleEntryByEventCodeQuery/{eventCode}", Name = "GetAccountingRuleEntryByEventCodeQuery")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingRuleEntryDto))]
        public async Task<IActionResult> GetAccountingRuleEntryByEventCodeQuery(string eventCode)
        {
            var getAccountingRuleEntryQuery = new GetAccountingRuleEntryByEventCodeQuery { EventCode = eventCode };
            var result = await _mediator.Send(getAccountingRuleEntryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All AccountingRuleEntries
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountingRuleEntry")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingRuleEntryDto>))]
        public async Task<IActionResult> GetAccountingRuleEntrys()
        {
            var getAllAccountingRuleEntryQuery = new GetAllAccountingRuleEntryQuery { };
            var result = await _mediator.Send(getAllAccountingRuleEntryQuery);
            return Ok(result);
        }

        /// <summary>
        /// AccountingEntry recorded for all other accounting Event FEE,EXPENSE,INCOME
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingEntry/OperationServicesAccountingRuleEntryQuery/{OpertionType}", Name = "OperationServicesAccountingRuleEntryQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<RuleEntryDto>))]
        public async Task<IActionResult> OperationServicesAccountingRuleEntryQuery(string OpertionType)
        {
            var model = new OperationServicesAccountingRuleEntryQuery { OpertionType = OpertionType };
            var result = await _mediator.Send(model);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get ChartAccount Number By ProductId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingRuleEntry/GetChartAccountNumberByProductId/{productId}", Name = "GetRootAccountNumberByProductIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(AccountMap))]
        public async Task<IActionResult> GetRootAccountNumberByProductIdQuery(string productId)
        {
            var model = new GetRootAccountNumberByProductIdQuery { ProductId = productId };
            var result = await _mediator.Send(model);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// load all Accounting Entry
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingEntry/LoadAccountingRuleEntryQuery", Name = "LoadAccountingRuleEntryQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<RuleEntries>))]
        public async Task<IActionResult> LoadAccountingRuleEntryQuery()
        {
            var model = new GetAccountingRuleEntryIdsQuery { };
            var result = await _mediator.Send(model);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All AccountingRuleEntryRuleIdsByAccountingRuleId
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("AccountingRuleEntry/GetAllAccountingRuleEntryRuleIdsByAccountingRuleId/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingEntryRuleIdsDto>))]
        public async Task<IActionResult> GetAccountingRuleEntryRuleIdsByAccountingRuleId(string Id)
        {
            var getAllAccountingRuleEntryQuery = new GetAllAccountingRuleEntryIdByAccountingRuleIdQuery { AccountingRuleId = Id };
            var result = await _mediator.Send(getAllAccountingRuleEntryQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a AccountingRuleEntry
        /// </summary>
        /// <param name="addAccountingRuleEntryCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingRuleEntry")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingRuleEntryDto))]
        public async Task<IActionResult> AddAccountingRuleEntry(AddAccountingRuleEntryCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update AccountingRuleEntry By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountingRuleEntryCommand"></param>
        /// <returns></returns>
        [HttpPut("AccountingRuleEntry/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingRuleEntryDto))]
        public async Task<IActionResult> UpdateAccountingRuleEntry(string Id, UpdateAccountingRuleEntryCommand updateAccountingRuleEntryCommand)
        {
            updateAccountingRuleEntryCommand.Id = Id;
            var result = await _mediator.Send(updateAccountingRuleEntryCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update AccountingRuleEntry position By AccountingRuleId
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountingRuleEntryCommand"></param>
        /// <returns></returns>
        [HttpPut("AccountingRuleEntry/UpdateAccountingEntryRulePositionCommand/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingRuleEntryDto))]
        public async Task<IActionResult> UpdateAccountingRuleEntryProsition(string Id, UpdateAccountingEntryRulePositionCommand updateAccountingRuleEntryCommand)
        {
            updateAccountingRuleEntryCommand.AccountingRuleId = Id;
            var result = await _mediator.Send(updateAccountingRuleEntryCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload AccountingRuleEntry from excel file
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uploadAccountingRuleEntry></param>
        /// <returns></returns>
        //[HttpPost("AccountingRuleEntry/UploadExcelFile/{Id}")]
        //[Produces("application/json", "application/xml", Type = typeof(Configurations))]
        //public async Task<IActionResult> UploadChartOfAccount(IFormFile form)
        //{
        //    UploadAccountingEntryRules uploadAccountingEntryRules = new UploadAccountingEntryRules();
        //    var configurations= new AccountingParams().ReadExcel(form);
        //    uploadAccountingEntryRules.AccountingRuleEntries = configurations.AccountingRuleEntries;
        //    uploadAccountingEntryRules.OperationEvents = configurations.OperationEvents;
        //    uploadAccountingEntryRules.OperationEventAttributes= configurations.OperationEventAttributes;
        //    var result = await _mediator.Send(uploadAccountingEntryRules);

        //    return ReturnFormattedResponse(result);
        //}

        /// <summary>
        /// Delete AccountingRuleEntry By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("AccountingRuleEntry/{Id}")]
        public async Task<IActionResult> DeleteAccountingRuleEntry(string Id)
        {
            var deleteAccountingRuleEntryCommand = new DeleteAccountingRuleEntryCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountingRuleEntryCommand);
            return ReturnFormattedResponse(result);
        }
    }
}