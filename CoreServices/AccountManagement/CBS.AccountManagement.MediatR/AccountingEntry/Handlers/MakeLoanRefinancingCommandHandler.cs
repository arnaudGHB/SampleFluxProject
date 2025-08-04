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
    public class MakeLoanRefinancingCommandHandler : IRequestHandler<MakeLoanRefinancingCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MakeLoanRefinancingCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;

        public CustomerInfo _customerInfo { get; set; } = new CustomerInfo();
        public MakeLoanRefinancingCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<MakeLoanRefinancingCommandHandler> logger,
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




        public async Task<ServiceResponse<bool>> Handle(MakeLoanRefinancingCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {

                Data.Account destinationAccount;
                Data.Account sourceAccount;
                Data.Account LiasonCreditAccount;
                Data.Account LiasonDebitAccount;
                List<Data.AccountingEntryDto> accountingEntries = new List<AccountingEntryDto>();
                List<Data.AccountingEntry> accountingEntry = new List<Data.AccountingEntry>();

                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.MakeLoanRefinancingCommand, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                //product account
                destinationAccount = await _accountingService.GetAccount(command.GetOperationEventCode(), _userInfoToken.BranchId, _userInfoToken.BranchCode);
                sourceAccount = await _accountingService.GetAccount(command.GetTransitOperationEventCode(), _userInfoToken.BranchId, _userInfoToken.BranchCode);
                accountingEntries.AddRange( await _accountingService.CashMovementAsync(command.Naration,command.MemberReference, command.TransactionDate,sourceAccount,destinationAccount,command.AmountToDisbursed, "LoanRefinancingPostingCommand",command.TransactionReferenceId, _userInfoToken.BranchId));
                var destinationAccount2 =  await _accountingService.GetAccount(command.GetPrimaryTeller(), _userInfoToken.BranchId, _userInfoToken.BranchCode);
                accountingEntries.AddRange(await _accountingService.CashMovementAsync(GenerateNaration(sourceAccount,destinationAccount,command), command.MemberReference, command.TransactionDate, sourceAccount, destinationAccount2, command.AmountToPayBackToCash, "LoanRefinancingPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));

                var CommissionSourceAccount = command.IsCommissionFromMember ? destinationAccount : await _accountingService.GetAccount(command.GetPrimaryTeller(), _userInfoToken.BranchId, _userInfoToken.BranchCode);
                foreach (var item in command.DisbursementCollections)
                {
                    var income_account = await _accountingService.GetAccount(item.EventCode, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                    accountingEntries.AddRange(await _accountingService.CashMovementAsync(item.Naration, command.MemberReference, command.TransactionDate, CommissionSourceAccount, income_account, item.Amount, "LoanRefinancingPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId));
                }
                accountingEntry = _mapper.Map(accountingEntries, accountingEntry);
                _accountingEntryRepository.AddRange(accountingEntry);
                if (_accountingService.EvaluateDoubleEntryRule(accountingEntry))
                {
                    await _uow.SaveAsyncWithOutAffectingBranchId();
                }
                else
                {
                      errorMessage = "Accounting double entry rule not validated contact administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.MakeLoanRefinancingCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return403(false,errorMessage);
                }
                errorMessage = "Transaction Completed Successfully";

                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.MakeLoanRefinancingCommand, LogLevelInfo.Information);


                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while performing account postings: {e.Message}";

                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.InternalServerError, LogAction.MakeLoanRefinancingCommand, LogLevelInfo.Error);

                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e, errorMessage);
            }
        }

        private string GenerateNaration(Data.Account sourceAccount, Data.Account destinationAccount, MakeLoanRefinancingCommand command)
        {
            return $"CASH IN(AmountToPayBackToCash during refinancing) of XAF {command.AmountToPayBackToCash} from {sourceAccount.AccountNumberCU}-{sourceAccount.AccountName} to " +
              $"{destinationAccount.AccountNumberCU}-{destinationAccount.AccountName} Ref:{command.TransactionReferenceId}";

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