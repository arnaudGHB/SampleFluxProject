using CBS.AccountManagement.API.Model;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.ChartOfAccount.MediatR.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Handlers;
using Microsoft.SqlServer.Server;
using CBS.AccountManagement.MediatR.Account.Commands;
namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// Account
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    //[Authorize]
    public class AccountController : BaseController
    {
        public IMediator _mediator { get; set; }
        public UserInfoToken _userInfoToken { get; set; }
        public PathHelper _pathHelper { get; set; }
        private readonly FileProcessor _fileProcessor;
        /// <summary>
        /// Account 
        /// </summary>
        /// <param name="mediator"></param>
        public AccountController(IMediator mediator,IWebHostEnvironment webHost, UserInfoToken userInfoToken, PathHelper pathHelper)
        {
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _pathHelper = pathHelper;
            _fileProcessor = new FileProcessor(webHost, pathHelper);
        }


        /// <summary>
        /// Get Account By Customer Reference
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/GetAccountByReference/{id}", Name = "GetAccountByReference")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAccountByReference(string id)
        {
            var getAccountQuery = new GetAccountByReferenceQuery() { referenceId = id };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account branch Gl with Tree display
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/GetAccountByAccountNumber/{CanPullAllAccount}", Name = "GetAllAccountAsTreeNodeQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<JsData>))]
        public async Task<IActionResult> GetAllAccountAsTreeNodeQuery(bool CanPullAllAccount)
        {
            var getAccountQuery = new GetAllAccountAsTreeNodeQuery() { CanPullAllAccount = CanPullAllAccount };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account By Customer Account Number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/GetAccountByAccountNumberRef/{id}", Name = "GetAccountByAccountNumber")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetAccountByAccountNumber(string id)
        {
            var getAccountQuery = new GetAccountByAccountNumberQuery() { AccountNumber = id };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account By Customer Account Number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/GetAccountByMFIBankAccountNumberQuery/{id}", Name = "GetAccountByMFIBankAccountNumberQuery")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetAccountByMFIBankAccountNumberQuery(string id)
        {
            var getAccountQuery = new GetAccountByMFIBankAccountNumberQuery() { BranchId = id };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account By Customer Account Number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/GetSystemLiaisonAccountQuery/{id}", Name = "GetSystemLiaisonAccountQuery")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetSystemLiaisonAccountQuery(string id)
        {
            var getAccountQuery = new GetSystemLiaisonAccountQuery() { BranchId = id };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/{id}", Name = "GetAccount")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetAccount(string id)
        {
            var getAccountQuery = new GetAccountQuery { Id = id };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All Accounts
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Accounts/LiaisonAccount")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAllLiasonAccountByBranchIdQuery()
        {
            var getAllAccountQuery = new GetAllLiasonAccountByBranchIdQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All Accounts
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Accounts")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAccounts()
        {
            var getAllAccountQuery = new GetAllAccountQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All Accounts By Braanche
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Accounts/{branchId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAccountsByBranch(string branchId)
        {
            var getAllAccountQuery = new GetAllAccountByBranchIDQuery { BranchId = branchId };
            var result = await _mediator.Send(getAllAccountQuery);
            return Ok(result);
        }

 
        [HttpPost("Account/UploadAccountCommand")]
        [Produces("application/json", "application/xml", Type = typeof(UploadAccountResult))]
        public async Task<IActionResult> UploadAccount(UploadAccount UploadAccountCommand)
        {
            var result = await _mediator.Send(UploadAccountCommand);
            if (result.Data==null)
            {
                return ReturnFormattedResponseObject(result);
            }
            if (result.Data.UploadStatus)
            {
                var command = new AddDocumentUploadedCommand
                {
                    //FormFiles = _fileProcessor.PrepareFileOnServer(true, new TrialBalanceUploadResult { AccountNotPresent =null, OriginalFile= UploadAccountCommand .AccountModelList}, ".xlsx", $"{result.Data.BranchName}_TrialBalance_" + BaseUtilities.ToUtc(DateTime.Now)),
                    IsSynchronus = true,
                    OperationID = Guid.NewGuid().ToString(),
                    DocumentType = "ALPHA VALIDATED TRIAL BALANCE",
                    ServiceType = "AccountingManagementService",
                    DocumentId = "N/A",
                    CallBackBaseUrl = "N/A",
                    CallBackEndPoint = "N/A",
                    RemoteFilePath = $"{_fileProcessor._pathHelper.FileUploadEndpointURL}"

                };
                var values =   command.GetFilesizeInKilobits();
                var model = new AddCommandTrialBalanceFile
                {
                    FilePath = result.Data.file_path,
                    Owner = result.Data.BranchName,
                    Size = values.ToString()+"KB",
                    Id = Guid.NewGuid().ToString()
                    



                };
                await _mediator.Send(model);
            }
      
  
            return ReturnFormattedResponseObject(result);
        }


        /// <summary>
        /// upload Account from file
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        [HttpPost("Account/UploadAccountForm")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UploadMOdelAccount(IFormFile formData)
        {
            UploadAccount UploadAccountCommand = new UploadAccount();
            UploadAccountCommand.AccountModelList = UploadAccountCommand.UploadAccountQueryModel(formData);

            var result = await _mediator.Send(UploadAccountCommand);
    
            return ReturnFormattedResponseObject(result); ;
        }

        /// <summary>
        /// upload a Accounts
        /// </summary>
        /// <param name="UploadAccount"></param>
        /// <returns></returns>
        [HttpPost("Account/UploadAccountCommandByForm")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> UploadAccountQueryModel(IFormFile form)
        {
            UploadAccountCommand UploadAccountCommand = new UploadAccountCommand();
            UploadAccountCommand.AccountModelList = UploadAccountCommand.UploadAccountQueryModel(form);
            var result = await _mediator.Send(UploadAccountCommand);
            return ReturnFormattedResponseObject(result);
        }

        /// <summary>
        /// Create a Account
        /// </summary>
        /// <param name="addAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("Account/ManualEntry")]
        [Produces("application/json", "application/xml", Type = typeof(AccountResponseDto))]
        public async Task<IActionResult> AddAccount(AddAccountMETCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponseObject(result);
        }
        /// <summary>
        /// Create a Account
        /// </summary>
        /// <param name="addAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("Account")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccount(AddAccountCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponseObject(result);
        }

        /// <summary>
        /// Create a Account
        /// </summary>
        /// <param name="addAccountCommandList"></param>
        /// <returns></returns>
        [HttpPost("Account/AccountList")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccount(AddAccountCommandList addAccountCommandList)
        {
            var result = await _mediator.Send(addAccountCommandList);
            return ReturnFormattedResponseObject(result);
        }

        /// <summary>
        /// Generate Branch Accounts
        /// </summary>
        /// <param name="GenerateBranchAccountQuery"></param>
        /// <returns></returns>
        [HttpPost("Account/GenerateBranchAccountQuery")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GenerateBankAccountFromChartOfAccount(GenerateBranchAccountQuery query)
        {
            var result = await _mediator.Send(query);
            return ReturnFormattedResponseObject(result);
        }


        /// <summary>
        /// Generate Branch Accounts
        /// </summary>
        /// <param name="GetAccountByEventCodeQuery"></param>
        /// <returns></returns>
        [HttpPost("Account/GetAccountByEventCodeQuery")]
        [Produces("application/json", "application/xml", Type = typeof(List<InfoAccount>))]
        public async Task<IActionResult> GetAccountByEventCodeQuery(GetAccountByEventCodeQuery query)
        {
            var result = await _mediator.Send(query);
            return ReturnFormattedResponseObject(result);
        }
        /// <summary>
        /// Upload all existing AccountCartegory
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("Accounts/UploadFiles/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountCartegoryDto>))]
        public async Task<IActionResult> UploadAccounts(IFormFile file)
        {
            var getAllOperationEventQuery = new UploadAccountQuery { Accounts = new AccountQueryModel().Upload(file) };
            var result = await _mediator.Send(getAllOperationEventQuery);
            return Ok(result);
        }

        /// <summary>
        /// Upload all existing AccountCartegory
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("Accounts/UploadAlphaAccounts/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> UploadAlphaAccounts(IFormFile file)
        {
            var getAllOperationEventQuery = new UploadAccountQuery { Accounts = new AccountQueryModel().Upload(file) };
            var result = await _mediator.Send(getAllOperationEventQuery);
            return Ok(result);
        }

        /// <summary>
        /// Update Account By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateAccountCommand"></param>
        /// <returns></returns>
        [HttpPut("Account/{Id}")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> UpdateAccount(string Id, UpdateAccountCommand updateAccountCommand)
        {
            updateAccountCommand.Id = Id;
            var result = await _mediator.Send(updateAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Delete Account By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Account/{Id}")]
        public async Task<IActionResult> DeleteAccount(string Id)
        {
            var deleteAccountCommand = new DeleteAccountCommand { Id = Id };
            var result = await _mediator.Send(deleteAccountCommand);
            return ReturnFormattedResponse(result);
        }
    }
}