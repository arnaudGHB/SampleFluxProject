using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using CBS.AccountManagement.Data.Dto.AccountingEntryDto;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.ExceptionHandler;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class BulkLoanRefundPostingCommandHandler : IRequestHandler<BulkLoanRefundPostingCommand, ServiceResponse<List<Data.AccountingEntry>>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BulkLoanRefundPostingCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;

   
        public BulkLoanRefundPostingCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<BulkLoanRefundPostingCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
            IChartOfAccountRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IMediator mediator, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _accountRepository = accountRepository;
            _operationEventAttributeRepository = operationEventAttributeRepository;
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _transactionDataRepository = transactionDataRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
            _chartOfAccountRepository = chartOfAccountRepository;
            _accountingRuleEntryRepository = accountingRuleEntryRepository;
            _mediator = mediator;
            _accountingService = accountingEntriesServices;
 
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
        }



 
        public async Task<ServiceResponse<List<Data.AccountingEntry>>> Handle(BulkLoanRefundPostingCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {

             
                List<Data.AccountingEntryDto> accountingEntries = new List<AccountingEntryDto>();
                List<Data.AccountingEntry> accountingEntry = new List<Data.AccountingEntry>();
                Data.Account FromProductAccount = new Data.Account();
                Data.Account VatAccount = new Data.Account();
                Data.Account InterestAccount = new Data.Account();
                Data.Account ToProductAccount = new Data.Account();
                //if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                //{
                //    errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                //    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.BulkLoanRefundPostingCommand, LogLevelInfo.Information);
                //    return ServiceResponse<List<Data.AccountingEntry>>.Return409(errorMessage);
                //}
                FromProductAccount = await _accountingService.GetAccountByProductID(command.FromProductId,"Saving", _userInfoToken.BranchId, _userInfoToken.BranchCode);
                if (command.IsOldSystemLoan)
                {                    
                 
                        ToProductAccount = await _accountingService.GetAccountByAccountNumberInsteadHavingId(command.LoanRefundCollectionAlpha.AmountAccountNumber, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    var entriesx = await _accountingService.CashMovementAsync(command.Naration,command.MemberReference, command.TransactionDate, FromProductAccount, ToProductAccount, Convert.ToDecimal(command.LoanRefundCollectionAlpha.AmountCapital), "BulkLoanRefundPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, false);
                        accountingEntries.AddRange(entriesx);
                        VatAccount = await _accountingService.GetAccountByAccountNumberInsteadHavingId(command.LoanRefundCollectionAlpha.VatAccountNumber, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                        var entries = await _accountingService.CashMovementAsync(command.LoanRefundCollectionAlpha.VatNaration,command.MemberReference, command.TransactionDate, FromProductAccount, VatAccount, Convert.ToDecimal(command.LoanRefundCollectionAlpha.AmountVAT), "BulkLoanRefundPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, false);
                        accountingEntries.AddRange(entries);
                        InterestAccount = await _accountingService.GetAccountByAccountNumberInsteadHavingId(command.LoanRefundCollectionAlpha.InterestAccountNumber, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                        var entriessd = await _accountingService.CashMovementAsync(command.LoanRefundCollectionAlpha.InterestNaration, command.MemberReference, command.TransactionDate, FromProductAccount, InterestAccount,Convert.ToDecimal( command.LoanRefundCollectionAlpha.AmountInterest), "BulkLoanRefundPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, false);
                        accountingEntries.AddRange(entriessd);
                     
                }
                else
                {
               
                    foreach (var item in command.AmountCollection)
                    {
                        if (item.Amount>0)
                        {
                            Data.Account Account = await _accountingService.GetAccount(item.GetOperationEventCode(command.LoanProductId), _userInfoToken.BranchId, _userInfoToken.BranchCode);
                            accountingEntries.AddRange(await _accountingService.CashMovementAsync(item.Naration, command.MemberReference, command.TransactionDate, FromProductAccount, Account, item.Amount, "MobileMoneyOperationCommand", command.TransactionReferenceId, _userInfoToken.BranchId));

                        }
                    }
                }
                accountingEntry = _mapper.Map(accountingEntries, accountingEntry);
       
                if (_accountingService.EvaluateDoubleEntryRule(accountingEntry))
                {
                    errorMessage = "Transaction Completed Successfully";

                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.BulkLoanRefundPostingCommand, LogLevelInfo.Information);

                }
                else
                {
                    errorMessage = "Accounting double entry rule not validated contact administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.BulkLoanRefundPostingCommand, LogLevelInfo.Warning);

                    return ServiceResponse<List<Data.AccountingEntry>>.Return403(null, errorMessage);
                }
              


                return ServiceResponse<List<Data.AccountingEntry>>.ReturnResultWith200(accountingEntry);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while performing account postings: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.MakeLoanRefinancingCommand, LogLevelInfo.Warning);

                _logger.LogError(errorMessage);
                return ServiceResponse<List<Data.AccountingEntry>>.Return500(e, errorMessage);
            }
        }
        private async Task<Data.Account> CreateLoanAccountAsync(string determinationAccountId, LoanDisbursementPostingCommand command)
        {

            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(determinationAccountId);
            chartOfAccount.ChartOfAccount = await _chartOfAccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);
            //var branch = await _accountingService.GetBranchCodeAsync(command.BranchId);
            AddAccountCommand commando = new AddAccountCommand
            {
                AccountName = chartOfAccount.Description + " " + _userInfoToken.BranchCode,
                AccountNumber = chartOfAccount.ChartOfAccount.AccountNumber,
                AccountNumberManagementPosition = chartOfAccount.PositionNumber.PadRight(3, '0'),
                AccountNumberNetwok = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BankCode + _userInfoToken.BranchCode).PadRight(12, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),
                AccountNumberCU = (chartOfAccount.ChartOfAccount.AccountNumber.PadRight(6, '0') + _userInfoToken.BranchCode).PadRight(9, '0') + chartOfAccount.PositionNumber.PadRight(3, '0'),
                AccountOwnerId = _userInfoToken.BranchId,
                AccountTypeId = "",
                ChartOfAccountManagementPositionId = chartOfAccount.Id,
                OwnerBranchCode = _userInfoToken.BranchCode,
                AccountCategoryId = chartOfAccount.ChartOfAccount.AccountCartegoryId,
            };
            await _mediator.Send(commando);
            var ProductFromAccount = _accountRepository.FindBy(x => x.ChartOfAccountManagementPositionId == determinationAccountId && x.BranchId == _userInfoToken.BranchId);
            if (ProductFromAccount.Any() == false)
            {

                string errorMessage = $"There is no destination account set for this transfer. Please kindly contact your administrator to contact system administrator";
                var exception = new Exception(errorMessage);
                LogLoanApprovalError(errorMessage, exception, command);
                throw exception;
            }

            return ProductFromAccount.FirstOrDefault();

        }
        private OperationEventAttributeTypes GetOperationEventAttributeTypes(string operationType)
        {
            if (operationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
            {
                return OperationEventAttributeTypes.deposit;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.withdrawal.ToString()))
            {
                return OperationEventAttributeTypes.withdrawal;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.transfer.ToString()))
            {
                return OperationEventAttributeTypes.transfer;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.cashreplenishment.ToString()))
            {
                return OperationEventAttributeTypes.cashreplenishment;
            }
            else
            {
                return OperationEventAttributeTypes.none;
            }
        }

        private TransactionCode GetTransCode(string operationType)
        {
            if (operationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
            {
                return TransactionCode.CINT;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.withdrawal.ToString()))
            {
                return TransactionCode.COUT;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.transfer.ToString()))
            {
                return TransactionCode.TRANS;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.cashreplenishment.ToString()))
            {
                return TransactionCode.CRP;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.loan_disbursement.ToString()))
            {
                return TransactionCode.LD;
            }
            else
            {
                return TransactionCode.None;
            }
        }


        private async Task LogError(string message, Exception ex, LoanDisbursementPostingCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(LoanDisbursementPostingCommand), command, message, "Error", 500, _userInfoToken.Token);
        }
        private async Task LogLoanApprovalError(string message, Exception ex, LoanDisbursementPostingCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(LoanDisbursementPostingCommand), command, message, "Error", 500, _userInfoToken.Token);
        }
        private async Task<OperationalObject> GetAmountFromCollectionAsync(EntryConfig model, string accountTypename)
        {

            if (model == null)
            {
                var errorMessage = $"There is no accounting rule entry with operationEventAttributeID: {model.OperationEventAttributeId} contact system administrator.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand.GetAmountFromCollection",
                    "", errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            var account = _accountingRuleEntryRepository.FindBy(b => b.OperationEventAttributeId.Equals(model.OperationEventAttributeId));
            if (account == null)
            {
                var errorMessage = $"There is no accounting rule entry with operationEventAttributeID: {model.OperationEventAttributeId} contact system administrator.";
                _logger.LogInformation(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand.GetAmountFromCollection",
                    "", errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                throw new Exception(errorMessage);
            }
            Data.AccountingRuleEntry accountingRuleEntry = account.FirstOrDefault();
            var accountModel = await _chartOfAccountRepository.FindAsync(accountingRuleEntry.DeterminationAccountId);
            var model1 = new OperationalObject
            {
                Amount = model.Amount,

                Account = accountModel,

            };
            return model1;
        }



        private string DetermineMessageDescription(string? eventCode, Data.Account Determination, Data.Account Balancing, string amount)
        {
            string message = string.Empty;
            if (eventCode.Equals("CASH_WITHDRAWAL"))
            {
                message = $"Withdrawal of XAF{amount} from {Determination.AccountName} to {Balancing.AccountName}";
            }
            else if (eventCode.Equals("CASH_DEPOSIT"))
            {
                message = $"Deposit of XAF{amount} into {Determination.AccountName} to {Balancing.AccountName}";
            }

            return message;
        }

        internal class OperationalObject
        {
            public decimal Amount { get; internal set; }

            public Data.ChartOfAccount Account { get; internal set; }

        }
    }


}