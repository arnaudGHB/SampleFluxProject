using CBS.AccountManagement.API.Model;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.ChartOfAccount.Commands;
using CBS.AccountManagement.MediatR.ChartOfAccount.Queries;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.ChartOfAccount.MediatR.Queries;
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
    public class ChartOfAccountController : BaseController
    {
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Account
        /// </summary>
        /// <param name="mediator"></param>
        public ChartOfAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get ChartOfAccount By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("ChartOfAccount/{Id}", Name = "GetChartOfAccount")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountDto))]
        public async Task<IActionResult> GetChartOfAccount(string Id)
        {
            var getAccountQuery = new GetChartOfAccountQuery { Id = Id };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get ChartOfAccount By Account Number
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("ChartOfAccount/GetChartOfAccountByAccountNumber/{Id}", Name = "GetChartOfAccountByAccountNumber")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountDto))]
        public async Task<IActionResult> GetChartOfAccountByAccountNumber(string Id)
        {
            var getAccountQuery = new GetChartOfAccountByAccountNumberQuery { AccountNumber = Id };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All ChartOfAccounts
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ChartOfAccounts")]
        [Produces("application/json", "application/xml", Type = typeof(List<ChartOfAccountDto>))]
        public async Task<IActionResult> GetAccounts()
        {
            var getAllAccountQuery = new GetAllChartOfAccountQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All ChartOfAccounts for JsTreeNode
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("ChartOfAccounts/JsTreeNode")]
        [Produces("application/json", "application/xml", Type = typeof(List<JsData>))]
        public async Task<IActionResult> GetJsTreeNodeAccounts()
        {
            var getAllAccountQuery = new GetAllChartOfAccountAsTreeNodeQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return Ok(result);
        }

        /// <summary>
        /// Create a ChartOfAccount
        /// </summary>
        /// <param name="addChartOfAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("ChartOfAccount")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountDto))]
        public async Task<IActionResult> AddAccount(AddChartOfAccountCommand addChartOfAccountCommand)
        {
            var result = await _mediator.Send(addChartOfAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update ChartOfAccount AccountClass base on AccountClassId
        /// </summary>
        /// <param name="Id">AccountClassId used for the update</param>
        /// <param name="updateAccountCommand"></param>
        /// <returns></returns>
        [HttpPut("ChartOfAccount/characteristicOfAccountChart/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<ChartOfAccountDto>))]
        public async Task<IActionResult> characteristicOfAccountChart(string Id, UpdateListOfChartOfAccountCommand updateAccountCommand)
        {
            updateAccountCommand.AccountClassId = Id;
            var result = await _mediator.Send(updateAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Update ChartOfAccount By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountCommand"></param>
        /// <returns></returns>
        [HttpPut("ChartOfAccount/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountDto))]
        public async Task<IActionResult> UpdateChartOfAccount(string Id, UpdateChartOfAccountCommand updateAccountCommand)
        {
            updateAccountCommand.Id = Id;
            var result = await _mediator.Send(updateAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload ChartOfAccount from excel file
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uploadChartOfAccountQuery"></param>
        /// <returns></returns>
        [HttpPost("ChartOfAccount/UploadChartOfAccount/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountDto))]
        public async Task<IActionResult> UploadChartOfAccount(IFormFile form)
        {
            var uploadModel = new UploadChartOfAccountQuery { ChartOfAccounts = new ChartOfAccountQueryModel().Upload(form, true) };
            var result = await _mediator.Send(uploadModel);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload ChartOfAccount from excel file
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uploadChartOfAccountQuery"></param>
        /// <returns></returns>
        [HttpPost("ChartOfAccount/UploadChartOfAccountQueryModel/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountDto))]
        public async Task<IActionResult> UploadChartOfAccountPabCCUL(IFormFile form)
        {
            var uploadModel = new UploadChartOfAccountQuery { ChartOfAccounts = new ChartOfAccountQueryModel().UploadChartOfAccountQueryModel(form, false) };
            var result = await _mediator.Send(uploadModel);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Upload ChartOfAccount English from excel file
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="uploadChartOfAccountQuery"></param>
        /// <returns></returns>
        [HttpPost("ChartOfAccount/UploadChartOfAccountEnglish/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(ChartOfAccountDto))]
        public async Task<IActionResult> UploadChartOfAccountEnglish(IFormFile form)
        {
            var uploadModel = new UploadChartOfAccountQuery { LanguageConvert = true, ChartOfAccounts = new ChartOfAccountQueryModel().Upload(form, true) };
            var result = await _mediator.Send(uploadModel);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete ChartOfAccount By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ChartOfAccount/{Id}")]
        public async Task<IActionResult> DeleteAccount(string Id)
        {
            var deleteAccountCommand = new DeleteChartOfAccountCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete ChartOfAccount By Account Number
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("ChartOfAccount/DeleteChartOfAccountByAccountNumber/{Id}")]
        public async Task<IActionResult> DeleteChartOfAccount(string Id)
        {
            var deleteAccountCommand = new DeleteChartOfAccountCommandWithAccountNumber() { Id = Id };
            var result = await _mediator.Send(deleteAccountCommand);
            return ReturnFormattedResponse(result);
        }
    }
}