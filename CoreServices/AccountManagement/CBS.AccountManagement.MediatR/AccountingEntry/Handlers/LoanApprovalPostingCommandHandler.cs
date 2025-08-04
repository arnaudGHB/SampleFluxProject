using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.AccountingEntryDto;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class LoanApprovalPostingCommandHandler : IRequestHandler<LoanApprovalPostingCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountRepository _chartOfAccountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanApprovalPostingCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        public readonly string TransitAccountNumber = "481";

        public CustomerInfo _customerInfo { get; set; } = new CustomerInfo();
        public LoanApprovalPostingCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<LoanApprovalPostingCommandHandler> logger,
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



        public async Task<ServiceResponse<bool>> Handle(LoanApprovalPostingCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {

                Data.Account CreditAccount;
                Data.Account DebitAccount;
                Data.Account LiasonCreditAccount;
                Data.Account LiasonDebitAccount;
                List<Data.AccountingEntryDto> accountingEntries = new List<AccountingEntryDto>();
                List<Data.AccountingEntry> accountingEntry = new List<Data.AccountingEntry>();
                Data.Account sourceAccount = new Data.Account();
                Data.Account destinationAccount = new Data.Account();
                if (await _accountingService.TransactionExists(command.TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref:{command.TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.LoanApprovalPostingCommand, LogLevelInfo.Information);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                Data.AccountingEntryDto CreditEntry;
                Data.AccountingEntryDto DebitEntry;

                destinationAccount = await _accountingService.GetAccount(command.GetOperationEventCode(), _userInfoToken.BranchId, _userInfoToken.BranchCode);

                sourceAccount = await _accountingService.GetAccount(command.GetTransitOperationEventCode(), _userInfoToken.BranchId, _userInfoToken.BranchCode);

                var entriesDto = await _accountingService.CashMovementAsync(command.Naration,command.MemberReference, BaseUtilities.UtcToLocal(), sourceAccount, destinationAccount, command.Amount, "LoanApprovalPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId);

                accountingEntries.AddRange(entriesDto);






                accountingEntry = _mapper.Map(accountingEntries, accountingEntry);
                // There transaction is taken down during disbursement
                //Data.TransactionData TransactionData = _accountingService.GenerateTransactionRecord(command.AccountNumber, GetOperationEventAttributeTypes(command.OperationType), GetTransCode(command.OperationType), command.TransactionReferenceId, "", GetPrincipal(command.amountCollection));
                //_transactionDataRepository.Add(TransactionData);
                _accountingEntryRepository.AddRange(accountingEntry);


                if (_accountingService.EvaluateDoubleEntryRule(accountingEntry))
                {
                    await _uow.SaveAsyncWithOutAffectingBranchId();
                }
                else
                {
                    errorMessage = "Accounting double entry rule not validated contact administrator";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.LoanApprovalPostingCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return403(false, errorMessage);
                }
                errorMessage = "Transaction Completed Successfully";

                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.OK, LogAction.LoanApprovalPostingCommand, LogLevelInfo.Information);


                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                errorMessage = $"Error occurred while performing account postings: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "MakeAccountPostingCommand",
                    command, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e, errorMessage);
            }
        }

        private async Task<Data.Account> CreateLoanAccountAsync(string determinationAccountId, LoanApprovalPostingCommand command)
        {

            var chartOfAccount = await _chartOfAccountManagementPositionRepository.FindAsync(determinationAccountId);
            chartOfAccount.ChartOfAccount = await _chartOfAccountRepository.FindAsync(chartOfAccount.ChartOfAccountId);
            var branch = await _accountingService.GetBranchCodeAsync(command.BranchId);
            AddAccountCommand commando = new AddAccountCommand
            {
                AccountName = chartOfAccount.Description + " " + branch.branchCode,
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
            else
            {
                return TransactionCode.None;
            }
        }
        private decimal GetPrincipal(List<AmountCollection> amountCollection)
        {
            return amountCollection.Where(x => x.IsPrincipal == true).FirstOrDefault().Amount;
        }

        private async Task LogError(string message, Exception ex, MakeAccountPostingCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(MakeAccountPostingCommand), command, message, "Error", 500, _userInfoToken.Token);
        }
        private async Task LogLoanApprovalError(string message, Exception ex, LoanApprovalPostingCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(MakeAccountPostingCommand), command, message, "Error", 500, _userInfoToken.Token);
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