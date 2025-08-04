using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Handlers;
using CBS.AccountManagement.MediatR.Queries;

using CBS.APICaller.Helper.LoginModel.Authenthication;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CBS.AccountManagement.API.Controllers
{
    /// <summary>
    /// AccountingEntry
    /// </summary>
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class AccountingEntryController : BaseController
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        public IMediator _mediator { get; set; }
        private readonly FileProcessor _fileProcessor;
        //private readonly POSContext _dbcontext;
 
       public AccountingEntryController(IMediator mediator, IWebHostEnvironment webHost, UserInfoToken userInfoToken, PathHelper pathHelper)
        {
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _pathHelper = pathHelper;
            _fileProcessor = new FileProcessor(webHost, pathHelper);
        }
        //public AccountingEntryController(IMediator mediator)
        //{
       
        //    _mediator = mediator;

        //}


        /// <summary>
        /// Get Account GL Entry record By AccountId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/JournalEntry", Name = "JournalEntry")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDetails))]
        public async Task<IActionResult> JournalEntry( GetJournalEntryQuery model)
        {
            ServiceResponse<ReportDto> commandResult = null;

            // Process the request
            var result = await _mediator.Send(model);
            if (result.StatusCode.Equals(200))
            {
                var modelRep = await _fileProcessor.ExportAccountingEntriesReportFileAsync(result.Data, "JournalEntry", "JournalEntry Account",result.Data.BranchName,_userInfoToken.FullName);
            

                var command = AddReportCommand.ConvertToReportCommand(modelRep);

                commandResult = await _mediator.Send(command);
            }

            return ReturnFormattedResponse(commandResult);
        }

        /// <summary>
        /// Get Account GL Entry record By AccountId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/GetAccountStatementEntry", Name = "GetAccountStatementEntry")]
        [Produces("application/json", "application/xml", Type = typeof(AccountDetails))]
        public async Task<IActionResult> GetAccountStatementEntry(GetAccountStatusQuery model)
        {
            ServiceResponse<ReportDto> commandResult = null;
            var result = await _mediator.Send(model);
            if (result.StatusCode.Equals(200))
            {
              var modelRep = await    _fileProcessor.ExportAccountingGeneralLedgerFileAsync(result.Data, "GL_Account","General Account Statement",result.Data.BranchName);
                var command =   AddReportCommand.ConvertToReportCommand(modelRep);
               
                  commandResult = await _mediator.Send(command);
            }
            return ReturnFormattedResponse(commandResult);
        }
        /// <summary>
        /// Get Account GL Entry record By AccountId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/GetAccountGLEntry", Name = "GetListAccountGLQuery")]
        [Produces("application/json", "application/xml", Type = typeof(ReportDto))]
        public async Task<IActionResult> GetListAccountGLQuery(GetListAccountGLQuery model)
        {
            ServiceResponse<ReportDto> commandResult = null;
            var result = await _mediator.Send(model);
            if (result.StatusCode.Equals(200))
            {
              var modelRep= await  _fileProcessor.ExportAccountingGeneralLedgerDetailsFileAsync(result.Data, "GL_Details","General Account Ledger", result.Data.BranchName);
                var command = AddReportCommand.ConvertToReportCommand(modelRep);

                  commandResult = await _mediator.Send(command);
            }
            return ReturnFormattedResponse(commandResult);
        }
        /// <summary>
        /// Get AccountToAccountTransferByTransctionCode record By TransctionCode
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingEntry/GetAccountToAccountTransferByTransctionCode/{id}", Name = "GetAccountToAccountTransferByTransctionCode")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> GetAccountToAccountTransferByTransctionCode(string id)
        {
            var getAccountingEntryQuery = new GetAccountToAccountTransferByTransctionCode { BulkExecutionCode = id };
            var result = await _mediator.Send(getAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get AccountingEntry record By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingEntry/{id}", Name = "GetAccountingEntry")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingEntryDto))]
        public async Task<IActionResult> GetAccountingEntry(string id)
        {
            var getAccountingEntryQuery = new GetAccountingEntryQuery { Id = id };
            var result = await _mediator.Send(getAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Get AccountingEntry record By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingEntry/GetBranchToBranchTransferDto/{id}", Name = "GetBranchToBranchTransferDto")]
        [Produces("application/json", "application/xml", Type = typeof(BranchToBranchTransferDto))]
        public async Task<IActionResult> GetBranchToBranchTransferDto(string id)
        {
            var getAccountingEntryQuery = new GetBranchToBranchTransferDto { Id = id };
            var result = await _mediator.Send(getAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }
        
        /// <summary>
        /// Get AccountingEntry record By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingEntries/{branchId}/{accountId}", Name = "GetAllAccountingEntry")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingEntryDto))]
        public async Task<IActionResult> GetAllAccountingEntry(string branchId, string accountId)
        {
            var getAccountingEntryQuery = new GetAllAccountingEntryQuery { BranchId = branchId, AccountId = accountId, FromDate = new DateTime(), ToDate = new DateTime() };
            var result = await _mediator.Send(getAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Get AccountingEntry record By TransactionReference Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("AccountingEntry/GetAccountingEntryByReferenceIdQuery/{id}", Name = "GetAccountingEntryByReferenceIdQuery")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingEntryDto))]
        public async Task<IActionResult> GetAccountingEntryByReferenceIdQuery(string id)
        {
            var getAccountingEntryQuery = new GetAccountingEntryByReferenceIdQuery { ReferenceId = id };
            var result = await _mediator.Send(getAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// AccountingEntry recorded for all other accounting Event
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[TransactionRequestTracker]
        [HttpPost("AccountingEntry/AutoPostingEventCommand", Name = "AutoPostingEventCommand")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingEntryDto))]
        public async Task<IActionResult> AutoPostingEventCommand(AutoPostingEventCommand model)
        {
            var result = await _mediator.Send(model);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// MakeNonCashAccountAdjustmentCommand enable user to adjust accounting recording with members account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[TransactionRequestTracker]
        [HttpPost("AccountingEntry/MakeNonCashAccountAdjustmentCommand", Name = "MakeNonCashAccountAdjustmentCommand")]
        [Produces("application/json", "application/xml", Type = typeof(AccountingEntryDto))]
        public async Task<IActionResult> MakeNonCashAccountAdjustmentCommand(MakeNonCashAccountAdjustmentCommand model)
        {
            var result = await _mediator.Send(model);
            return ReturnFormattedResponse(result);
        }
        
        /// <summary>
        /// Reverse AccountingEntry record
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/ReverseAccountingEntry", Name = "ReverseAccountingEntry")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> ReverseAccountingEntry(ReverseAccountingEntryCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// GenerateLedger of account entries for a bank,branch.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the list of liaison account movement</response>
        [HttpPost("AccountingEntries/RetrieveEntries")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingEntryDto>))]
        public async Task<IActionResult> GetAccountingEntries(GetAllAccountingEntryQuery getAllAccountingEntryQuery)
        {
            var result = await _mediator.Send(getAllAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GenerateLiaisonEntries accounting entries for a bank,branch.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the list of liaison account movement</response>
        [HttpPost("AccountingEntries/LiaisonEntries")]
        [Produces("application/json", "application/xml", Type = typeof(List<LiaisonLedgerEntry>))]
        public async Task<IActionResult> GetLiaisonEntries(GetLiaisonLedgerEntryQuery LiaisonLedgerEntry)
        {
            var result = await _mediator.Send(LiaisonLedgerEntry);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GenerateBranchLiaisonEntries accounting entries for a bank,branch.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the list of liaison account movement for a branch</response>
        [HttpPost("AccountingEntries/BranchLiaisonEntries")]
        [Produces("application/json", "application/xml", Type = typeof(List<BranchLiaisonLedgerEntry>))]
        public async Task<IActionResult> GetBranchLiaisonEntries(GetBranchLiaisonLedgerEntryQuery BranchLiaisonLedgerEntry)
        {
            var result = await _mediator.Send(BranchLiaisonLedgerEntry);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Generate4ColumnTrialBalance accounting entries for a bank,branch.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountingEntries/Generate4ColumnTrialBalance")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingEntryDto>))]
        public async Task<IActionResult> GetAccountingEntriesForBranch(Get4ColumnTrialBalanceQuery getAllAccountingEntryQuery)
        {
            ServiceResponse<ReportDto> commandResult = null;
            var result = await _mediator.Send(getAllAccountingEntryQuery);
            if (result.StatusCode.Equals(200))
            {
                if (result.Data.Count() > 0)
                {
                    var modelRep = await _fileProcessor.Export4ColumnTrialBalanceAsync(result.Data, $"TB4C_{result.Data[0].BranchName}", "TrialBalance4C", _userInfoToken.FullName, result.Data[0].BranchName);
                    var command = AddReportCommand.ConvertToReportCommand(modelRep);

                    commandResult = await _mediator.Send(command);
                    return ReturnFormattedResponse(commandResult);
                }
                return ReturnFormattedResponse(result);
            }
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Generate6ColumnTrialBalance accounting entries for a bank,branch.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountingEntries/Generate6ColumnTrialBalance")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingEntryDto>))]
        public async Task<IActionResult> GetAccountingEntriesForBranch(Get6ColumnTrialBalanceQuery getAllAccountingEntryQuery)
        {
            ServiceResponse<ReportDto> commandResult = null;
            var result = await _mediator.Send(getAllAccountingEntryQuery);
            if (result.StatusCode.Equals(200))
            {
                if (result.Data.Count()>0)
                {
                    var modelRep = await _fileProcessor.Export6ColumnTrialBalanceAsync(result.Data, $"TB6C_{result.Data[0].BranchName}", "TrialBalance6C", _userInfoToken.FullName, result.Data[0].BranchName);
                    var command = AddReportCommand.ConvertToReportCommand(modelRep);

                    commandResult = await _mediator.Send(command);
                    return ReturnFormattedResponse(commandResult);
                }
                return ReturnFormattedResponse(result);
            }
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// PostAutomatedEventEntryCommand accounting entries for a bank,branch.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountingEntries/PostAutomatedEventEntryCommand")]
        [Produces("application/json", "application/xml", Type = typeof(EventEntryResponse))]
        public async Task<IActionResult> PostAutomatedEventEntryCommand(AutomatedEventEntryCommand getAllAccountingEntryQuery)
        {
            var result = await _mediator.Send(getAllAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Generate4IntermediateColumnTrialBalance accounting entries for a bank,branch.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountingEntries/Generate4IntermediateColumnTrialBalance")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingEntryDto>))]
        public async Task<IActionResult> GetAccountingIntermediateEntriesForBranch(Get4ColumnIntermediateTrialBalanceQuery getAllAccountingEntryQuery)
        {
            var result = await _mediator.Send(getAllAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Generate6IntermediateColumnTrialBalance accounting entries for a bank,branch.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountingEntries/Generate6IntermediateColumnTrialBalance")]
        [Produces("application/json", "application/xml", Type = typeof(List<AccountingEntryDto>))]
        public async Task<IActionResult> GetAccountingIntermediateEntriesForBranch(Get6ColumnIntermediateTrialBalanceQuery getAllAccountingEntryQuery)
        {
            var result = await _mediator.Send(getAllAccountingEntryQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GenerateProfitAndLossStatement As per type.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountingEntries/GenerateProfitAndLossStatement")]
        [Produces("application/json", "application/xml", Type = typeof(List<IncomeExpenseStatementDto>))]
        public async Task<IActionResult> GenerateProfitAndLossStatement(GetProfitAndLossStatementQuery getProfitAndLossStatementQuery)
        {
            var result = await _mediator.Send(getProfitAndLossStatementQuery);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// GenerateProfitAndLossStatement As per type.
        /// </summary>
        /// <returns>Test</returns>
        /// <response code="200">Returns the newly created item</response>
        [HttpPost("AccountingEntries/GenerateBalanceSheet")]
        [Produces("application/json", "application/xml", Type = typeof(List<IncomeExpenseStatementDto>))]
        public async Task<IActionResult> GenerateBalanceSheet(GetGeneralBalanceSheetQuery getGeneralBalanceSheetQuery)
        {
            var result = await _mediator.Send(getGeneralBalanceSheetQuery);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// CashInitializationCommand initialises  one branch vault to cash in hand
        /// </summary>
        /// <param name="CashInitializationCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/CashInitializationCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> CashRequisitionCommand(CashInitializationCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// InternalAutoPostingEventCommand event used to post all sort of entries
        /// </summary>
        /// <param name="CashInitializationCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/InternalAutoPostingEventCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> InternalAutoPostingEventCommand(InternalAutoPostingEventCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// CashRequisitionCommand Transfer money from one branch vault to cash in hand
        /// </summary>
        /// <param name="CashRequisitionCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/CashRequisitionCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> CashRequisitionCommand(CashRequisitionCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Cash Clearing Transfer money from one branch vault to another branch  liaison
        /// </summary>
        /// <param name="CashClearingTransferCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/CashClearingTransferFromCashReplenishmentCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> CashClearingTransferCommand(CashClearingTransferFromCashReplenishmentCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Cash Clearing Transfer money from one branch vault to another branch  liaison
        /// </summary>
        /// <param name="CashClearingTransferCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/CashClearingTransferFromBankDepositCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> CashClearingTransferCommand(CashClearingTransferFromBankDepositCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Transfering money from one branch vault to another branch  liaison
        /// </summary>
        /// <param name="BranchToBranchTransferCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/BranchToBranchTransferCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> BranchToBranchTransferCommand(BranchToBranchTransferCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }
        
        /// <summary>
        /// Cleaning of data for testing 
        /// </summary>
        /// <param name="CleanAccountAndAccountingEntriesCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/CleanAccountAndAccountingEntriesCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> CleanAccountAndAccountingEntriesCommand(CleanAccountAndAccountingEntriesCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Posting of transaction in the General Ledger
        /// </summary>
        /// <param name="MakeAccountPostingCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeAccountPosting")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(MakeAccountPostingCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of MomoCollection transaction in the General Ledger
        /// </summary>
        /// <param name="MakeAccountPostingCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MomoMobileMoney")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(MobileMoneyOperationCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of MomoCollection transaction in the General Ledger
        /// </summary>
        /// <param name="MobileMoneyCollectionOperationCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MomoCollection")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(MobileMoneyCollectionOperationCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of MobileMoney Management transaction in the General Ledger
        /// </summary>
        /// <param name="MobileMoneyCollectionOperationCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MobileMoneyManagement")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(MobileMoneyManagementPostingCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of DailyCollection transaction posting in the General Ledger
        /// </summary>
        /// <param name="DailyCollectionPostingEventCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/DailyCollectionPostingEvent")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddDailyCollectionPostingEventCommand(DailyCollectionPostingEventCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of DailyCollection Confirmation Posting  in the General Ledger
        /// </summary>
        /// <param name="DailyCollectionConfirmationPostingEvent"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/DailyCollectionConfirmationPostingEvent")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddDailyCollectionPostingEventCommand(DailyCollectionConfirmationPostingEventCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of DailyCollection monthly commision Posting  in the General Ledger
        /// </summary>
        /// <param name="DailyCollectionMonthlyCommisionEvent"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/DailyCollectionMonthlyCommisionEvent")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> DailyCollectionMonthlyCommisionEventCommand(DailyCollectionMonthlyCommisionEventCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of DailyCollection Confirmation Posting  in the General Ledger
        /// </summary>
        /// <param name="DailyCollectionMonthlyPayableEventCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/DailyCollectionMonthlyPayableEvent")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> DailyCollectionMonthlyPayableEventCommand(DailyCollectionMonthlyPayableEventCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Posting of Remittance in the General Ledger
        /// </summary>
        /// <param name="MakeRemittanceCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeRemittanceCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddBulkAccountingEntry(MakeRemittanceCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }


        /// <summary>
        /// Posting of transaction in the General Ledger
        /// </summary>
        /// <param name="MakeAccountPostingCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeBulkAccountPosting")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddBulkAccountingEntry(MakeBulkAccountPostingCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Loan Approval Posting Command of transaction in the General Ledger
        /// </summary>
        /// <param name="LoanApprovalPostingCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeLoanApprovalPosting")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(LoanApprovalPostingCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Loan Disbursement Posting Command of transaction in the General Ledger
        /// </summary>
        /// <param name="LoanDisbursementPostingCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeLoanDisbursementPosting")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(LoanDisbursementPostingCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Loan Disbursement Posting Command of transaction in the General Ledger
        /// </summary>
        /// <param name="LoanDisbursementPostingCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/LoanRefinancingPosting")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> LoanRefinancingPosting(MakeLoanRefinancingCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }
        /// <summary>
        /// Loan Refund Posting Command for all transaction in the General Ledger in momocash collection
        /// </summary>
        /// <param name="LoanDisbursementPostingCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeLoanRefundPosting")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(LoanRefundPostingCommand command)
        {
            var result = await _mediator.Send(command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of Transfer Event into the ledger
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeTransferPosting")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(AddTransferEventCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of Transfer to non memeber event into ledger either from teller or member
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeTransferPosting/ToNonMember")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(AddTransferToNonMemberEventCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of Transfer to non memeber event into ledger either from teller or member
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/MakeTransferPostingforWithDrawalSameBranch")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> AddAccountingEntry(AddTransferWithdrawalEventCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Posting of Opening Of the Day Accounting Event into the ledger
        /// { "OOD", "NEGATIVE_OOD","POSITIVE_OOD","COD","NEGATIVE_COD", "POSITIVE_COD"}
        /// </summary>
        /// <param name="OpeningOfDayPostingEventCommand"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/OpeningAndClosingOfDayPostingEventPosting")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> OpenOfDayPostingEventCommandEntry(OpeningOfDayEventCommand addAccountCommand)
        {
            var result = await _mediator.Send(addAccountCommand);
            return ReturnFormattedResponse(result);
        }

  

        /// <summary>
        /// Posting of Closing member Accounting Event into the ledger
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        [HttpPost("AccountingEntry/ClosingOfMemberAccountCommand")]
        [Produces("application/json", "application/xml", Type = typeof(bool))]
        public async Task<IActionResult> ClosingOfMemberAccountCommandEntry(ClosingOfMemberAccountCommand Command)
        {
            var result = await _mediator.Send(Command);
            return ReturnFormattedResponse(result);
        }

        /// <summary>
        /// Cancel Posting of Accounting Entries By Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("AccountingEntry/{referenceId}")]
        public async Task<IActionResult> DeleteAccountingEntry(string referenceId)
        {
            var deleteAccountingEntryCommand = new DeleteAccountPostingCommand { referenceId = referenceId };
            var result = await _mediator.Send(deleteAccountingEntryCommand);
            return ReturnFormattedResponse(result);
        }
    }
}