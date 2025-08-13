using CBS.TransactionManagement.Helper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Handlers;
using CBS.TransactionManagement.Queries;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CBS.TransactionManagement.MediatR.GAV.Query;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.MediatR.Queries;
using CBS.TransactionManagement.Data.Dto.Resource;
using CBS.TransactionManagement.MediatR.Commands;

namespace CBS.TransactionManagement.API.Controllers
{
    /// <summary>
    /// Account
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountController : BaseController
    {
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Account
        /// </summary>
        /// <param name="mediator"></param>

        private readonly ILogger<AccountController> _logger;
        public AccountController(IMediator mediator, ILogger<AccountController> logger = null)
        {
            _mediator = mediator;
            _logger = logger;
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
        /// Update Loan Account Balance After Interest Calculation
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="updateMemberAccountActivationCommand"></param>
        /// <returns></returns>
        [HttpPost("Account/UpdateLoanAccountBalanceInterestCalculation")]
        [Produces("application/json", "application/xml", Type = typeof(MemberAccountActivationDto))]
        public async Task<IActionResult> UpdateLoanAccountBalanceInterestCalculation(UpdateLoanAccountBalanceCommand updateMemberAccountActivationCommand)
        {
            // Start the processing of the command
            var result = await _mediator.Send(updateMemberAccountActivationCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves all remittance accounts by using branch id.
        /// </summary>
        /// <param name="query">Query parameters for retrieving remittance accounts.</param>
        /// <returns>Returns a list of remittance accounts matching the criteria in JSON or XML format.</returns>
        [HttpGet("Remittance/AccountsAsPerBranch/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAllRemittanceAccounts(string id)
        {
            // Send the query to the mediator to retrieve remittance accounts
            var result = await _mediator.Send(new GetAllRemittanceAccountQuery { BranchId=id});

            // Return the response in the desired format
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves a remittance account based on AccountType and BranchId.
        /// </summary>
        /// <param name="accountType">The type of the account to retrieve.</param>
        /// <param name="branchId">The branch ID associated with the account.</param>
        /// <returns>Returns the remittance account details in JSON or XML format.</returns>
        [HttpPost("Remittance/Account")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetRemittanceAccountByTypeAndBranch([FromBody] GetRemittanceAccountByTypeQuery query)
        {
            var result = await _mediator.Send(query);

            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account By SenderAccountNumber
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpGet("Account/SenderAccountNumber/{accountNumber}", Name = "GetAccountByAccountNumber")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetAccountByAccountNumber(string accountNumber)
        {
            var getAccountQuery = new GetAccountByAccountNumberQuery { AccountNumber = accountNumber };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Retrieves a paginated list of all member accounts based on the specified criteria.
        /// </summary>
        /// <param name="accountResource">The resource containing pagination and filter criteria.</param>
        /// <returns>An IActionResult containing the paginated member account summaries.</returns>
        [HttpGet("Account/Pagginated/MembersAccountSummary", Name = "GetAllMembersAccountPegginationQuery")]
        [Produces("application/json", "application/xml", Type = typeof(MemberAccountSituationListing))]
        public async Task<IActionResult> GetAllMembersAccountPegginationQuery([FromQuery] AccountResource accountResource)
        {
            // Create a query object with the provided accountResource
            var getAccountQuery = new GetAllMembersAccountPegginationQuery { AccountResource = accountResource };
            // Send the query to the mediator to get the result
            var result = await _mediator.Send(getAccountQuery);
            if (result.Data != null)
            {
                var paginationMetadata = new
                {
                    totalCount = result.Data.TotalCount,
                    pageSize = result.Data.PageSize,
                    skip = result.Data.Skip,
                    totalPages = result.Data.TotalPages
                };
                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            }

            // Return the formatted response
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account By teller Id
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        [HttpPost("Account/GetTellerAccount", Name = "GetTellerAccountBalanceQuery")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetTellerAccountBalanceQuery(GetTellerAccountBalanceQuery getTellerAccountBalance)
        {
            var result = await _mediator.Send(getTellerAccountBalance);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get Virtual Teller Account By Branch Id and TellerType
        /// </summary>
        /// <param name="branchid"></param>
        /// <param name="tellerType"></param>
        /// <returns></returns>
        [HttpPost("Account/GetVirtualTellerAccount", Name = "GetVirtualTellerAccountBalanceQuery")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetVirtualTellerAccountBalanceQuery(GetVirtualTellerAccountBalanceQuery getVirtualTellerAccountBalanceQuery)
        {
            var result = await _mediator.Send(getVirtualTellerAccountBalanceQuery);
            return ReturnFormattedResponse(result);
        }
        //GetTellerAccountBalanceQuery
        /// <summary>
        /// Get customer by SenderAccountNumber
        /// </summary>
        /// <param name="getCustomerByAccountNumber"></param>
        /// <returns></returns>
        [HttpGet("Account/AccountHolder/{accountNumber}", Name = "GetCustomerByAccountNumber")]
        [Produces("application/json", "application/xml", Type = typeof(CustomerKYCDto))]
        public async Task<IActionResult> GetCustomerByAccountNumber(string accountNumber)
        {
            var getAccountQuery = new GetCustomerByAccountNumber { AccountNumber = accountNumber };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account By Status
        /// </summary>
        /// <param name="Status"></param>
        /// <returns></returns>
        [HttpGet("Account/Status/{Status}", Name = "GetAccountSByStatus")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> GetAccountsByStatus(string Status)
        {
            var getAccountQuery = new GetAllAccountByStatusQuery { Status = Status };
            var result = await _mediator.Send(getAccountQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get Account Balance By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/Balance/{id}", Name = "GetAccountBalance")]
        [Produces("application/json", "application/xml", Type = typeof(AccountBalanceDto))]
        public async Task<IActionResult> GetAccountBalance(string id)
        {
            var getAccountBalanceQuery = new GetAccountBalanceQuery { Id = id };
            var result = await _mediator.Send(getAccountBalanceQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get account balance by account number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/GetBalanceThirdPartyCommand/{id}", Name = "GetBalanceThirdPartyCommand")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountBalanceThirdPartyDto>))]
        public async Task<IActionResult> GetBalanceThirdPartyCommand(string id)
        {
            var getAllAccountsBalanceQuery = new GetMemberAccountBalanceTTPQuery { AccountNumber = id };
            var result = await _mediator.Send(getAllAccountsBalanceQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All Accounts By CustomerId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("Account/Balance/Customer/{id}", Name = "GetAccountBalanceByCustomer")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountBalanceDto>))]
        public async Task<IActionResult> GetAccountBalanceByCustomer(string id)
        {
            var getAllAccountsBalanceQuery = new GetAllAccountsBalanceQuery { CustomerId = id };
            var result = await _mediator.Send(getAllAccountsBalanceQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get All Account
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Account")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAccounts()
        {
            var getAllAccountQuery = new GetAllAccountQuery { };
            var result = await _mediator.Send(getAllAccountQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All Account short
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the all items</response>
        [HttpGet("Account/short")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountShortDto>))]
        public async Task<IActionResult> GetAllAccountsShort()
        {
            var getAllAccountShortQuery = new GetAllAccountShortQuery { };
            var result = await _mediator.Send(getAllAccountShortQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All Customer Accounts
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Account/Customer/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAccountsByCustomer(string id)
        {
            var getAllAccountByCustomerIdQuery = new GetAllAccountByCustomerIdQuery { CustomerId = id };
            var result = await _mediator.Send(getAllAccountByCustomerIdQuery);
            return Ok(result);
        }


        /// <summary>
        /// Get All Customer Accounts, short version
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Account/Customer/short/{id}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountShortDto>))]
        public async Task<IActionResult> GetAccountsByCustomerShort(string id)
        {
            var getAllAccountShortByCustomerIdQuery = new GetAllAccountShortByCustomerIdQuery { CustomerId = id };
            var result = await _mediator.Send(getAllAccountShortByCustomerIdQuery);
            return Ok(result);
        }

        /// <summary>
        /// Get All Customer Accounts, short version
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpGet("Account/Tellers/{bankId}/{BranchId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAllTellerAccounts(string bankId, string branchId)
        {
            var GetAllTellerAccountsByBankIdAndBranchId = new GetAllTellerAccountsByBankIdAndBranchId { BankId = bankId, BranchId = branchId };
            var result = await _mediator.Send(GetAllTellerAccountsByBankIdAndBranchId);
            return Ok(result);

        }

        [HttpGet("Account/GetAllAccountsByBranchIdQuery/{BranchId}")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountDto>))]
        public async Task<IActionResult> GetAllCustomerAccounts(string branchId)
        {
            var GetAllCustomerAccountsByBankIdAndBranchId = new GetAllAccountsByBranchIdQuery { BranchId = branchId };
            var result = await _mediator.Send(GetAllCustomerAccountsByBankIdAndBranchId);
            return ReturnFormattedResponse(result);

        }

        /// <summary>
        /// Create a Account
        /// </summary>
        /// <param name="addAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("Account")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> AddAccount(AddAccountCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Create default accounts
        /// </summary>
        /// <param name="addDefaultAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("Account/DefaultAccount")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddDefaultAccountCommand(AddDefaultAccountCommand addDefaultAccountCommand)
        {
            var result = await _mediator.Send(addDefaultAccountCommand);
            return ReturnFormattedResponse(result);
        }
        ////AddDefaultAccountCommand
        ///// <summary>
        ///// Migrate Members Individual Accounts
        ///// </summary>
        ///// <param name="accountMigrationCommand"></param>
        ///// <returns></returns>
        //[HttpPost("Account/AccountMigration")]
        //[Produces("application/json", "application/xml", Type = typeof(bool))]
        //public async Task<IActionResult> AccountMigrationCommand(AccountMigrationCommand accountMigrationCommand)
        //{
        //    var result = await _mediator.Send(accountMigrationCommand);
        //    return ReturnFormattedResponse(result);
        //}

        /// <summary>
        /// Migrate Members Individual Accounts
        /// </summary>
        /// <param name="accountMigrationCommand"></param>
        /// <returns></returns>
        [HttpPost("Account/AccountMigration")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AccountMigrationCommand(AccountMigrationCommand accountMigrationCommand)
        {
            try
            {
                var result = await _mediator.Send(accountMigrationCommand);
                return ReturnFormattedResponse(result);
                //Enqueue the background job to run after 30 seconds
                //BackgroundJob.Schedule(() => ProcessBulkUpload(accountMigrationCommand), TimeSpan.FromSeconds(30));
                //Respond immediately to the client
                //return Ok(ServiceResponse<bool>.ReturnResultWith200(true, result.Result.Message));
                //return Ok(ServiceResponse<bool>.ReturnResultWith200(true, "Members account balance upload is initiated successfully and will run in the background. Process will teminate within the next 10-Mins"));
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"Failed to initiate batch upload process: {ex.Message}");
                // Respond with false and an error message
                return BadRequest(ServiceResponse<bool>.Return500($"Failed to initiate batch upload process: {ex.Message}"));
            }
        }


        // This method will be executed in the background
        [ApiExplorerSettings(IgnoreApi = true)] // Add this attribute to exclude the method from Swagger
        public async Task ProcessBulkUpload(AccountMigrationCommand addFileDownloadInfoCommand)
        {
            var result = await _mediator.Send(addFileDownloadInfoCommand);
            // Handle the result if needed
        }



        /// <summary>
        /// Create a loan account for members who are requesting for loan account during loan application.
        /// </summary>
        /// <param name="addLoanAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("Account/AddLoanAccount")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddLoanAccount(AddLoanAccountCommand addLoanAccountCommand)
        {
            var result = await _mediator.Send(addLoanAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Blocks a list of account balances for members who have an active loan application. 
        /// This method processes a list of accounts to block a specified amount, records the reason for blocking, 
        /// and creates a blocked account entry for each account in the request.
        /// </summary>
        /// <param name="blockListOfAccountBalanceCommand">Command containing the list of accounts to be blocked, 
        /// along with the blocked amount, reason, and loan application details.</param>
        /// <returns>Returns an IActionResult containing the formatted response (success or failure) 
        /// based on the operation result.</returns>
        [HttpPost("Account/BlockListOfAccountBalance")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> BlockListOfAccountBalance(BlockListOfAccountBalanceCommand blockListOfAccountBalanceCommand)
        {
            var result = await _mediator.Send(blockListOfAccountBalanceCommand);
            return ReturnFormattedResponse(result);
        }

        //BlockListOfAccountBalanceCommand
        /// <summary>
        /// Crediting member's loan account during loan approval.
        /// </summary>
        /// <param name="creditLoanAccountCommand"></param>
        /// <returns></returns>
        [HttpPost("Account/CreditLoanAccount")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> CreditLoanAccount(CreditLoanAccountCommand creditLoanAccountCommand)
        {
            var result = await _mediator.Send(creditLoanAccountCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Update Account Status By account number
        /// </summary>
        /// <param name="AccountNumber"></param>
        /// <param name="updateAccountStatusCommand"></param>
        /// <returns></returns>
        [HttpPut("Account/UpdateStatus/{SenderAccountNumber}")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDto))]
        public async Task<IActionResult> UpdateAccount(string AccountNumber, UpdateAccountStatusCommand updateAccountStatusCommand)
        {
            updateAccountStatusCommand.AccountNumber = AccountNumber;
            var result = await _mediator.Send(updateAccountStatusCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Delete Account By Id
        /// </summary>
        /// <param name="id">Account Id</param>
        /// <returns>Result of deletion</returns>
        [HttpDelete("Account/{id}")]
        public async Task<IActionResult> DeleteAccount(string id)
        {
            var deleteAccountCommand = new DeleteAccountCommand(id);
            var result = await _mediator.Send(deleteAccountCommand);
            return ReturnFormattedResponse(result);
        }
    }
}
