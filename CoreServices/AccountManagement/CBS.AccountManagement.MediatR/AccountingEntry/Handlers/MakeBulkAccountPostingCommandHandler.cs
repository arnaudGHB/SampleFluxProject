using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Azure;

namespace CBS.AccountManagement.MediatR.Handlers
{
    #region Class Definition
    // Handles the command for bulk account postings
    public class MakeBulkAccountPostingCommandHandler : IRequestHandler<MakeBulkAccountPostingCommand, ServiceResponse<bool>>
    {
        #region Fields
        // Dependency injections for required repositories, services, and utilities
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountRepository;
        private readonly IAccountingRuleEntryRepository _accountingRuleEntryRepository;
        private readonly IOperationEventAttributeRepository _operationEventAttributeRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<MakeBulkAccountPostingCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        #endregion

        #region Properties
        // Holds customer information
        public CustomerInfo _customerInfo { get; set; } = new CustomerInfo();
        #endregion

        #region Constructor
        // Constructor initializes the injected dependencies
        public MakeBulkAccountPostingCommandHandler(
            ITransactionDataRepository transactionDataRepository,
            IAccountingEntryRepository accountingEntryRepository,
            IOperationEventAttributeRepository operationEventAttributeRepository,
            IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
            IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<MakeBulkAccountPostingCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            UserInfoToken userInfoToken,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository,
            IAccountingEntriesServices accountingEntriesServices,
            IMediator mediator)
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
        }
        #endregion

        #region Methods
        // Main handler method for bulk account postings
        public async Task<ServiceResponse<bool>> Handle(MakeBulkAccountPostingCommand command, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty; 
            try
            {
                // Check for duplicate transactions
                if (await _accountingService.TransactionExists(command.MakeAccountPostingCommands[0].TransactionReferenceId))
                {
                     errorMessage = $"An entry has already been posted with this transaction Ref:{command.MakeAccountPostingCommands[0].TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Conflict, LogAction.MakeBulkAccountPostingCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                // Process the transactions and generate accounting entries
                var accountingEntries = await ProcessTransactions(command.MakeAccountPostingCommands);

                // Validate and save the accounting entries
                if (!await ValidateAndSaveEntries(accountingEntries))
                {
                    errorMessage = $"Accounting double entry rule not validated contact administrator.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.MakeBulkAccountPostingCommand, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                errorMessage = "Transaction Completed Successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage, command, HttpStatusCodeEnum.Forbidden, LogAction.MakeBulkAccountPostingCommand, LogLevelInfo.Warning);


                // Return success response if no issues
                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception ex)
            {
                // Handle exceptions and return failure response
                return ServiceResponse<bool>.Return403(false, ex.Message);
            }
        }

        #region Helper Methods


        // Process multiple transactions to generate accounting entries
        private async Task<List<Data.AccountingEntry>> ProcessTransactions(IEnumerable<MakeAccountPostingCommand> commands)
        {
            var allEntries = new List<Data.AccountingEntry>();
            bool HasPaidCommissionByCash = commands.ToArray()[0].AmountCollection[0].HasPaidCommissionByCash;
            foreach (var command in commands)
            {
                if (command.ProductId.Equals("126110052827546"))
                {

                }
                // Process individual transaction and add entries
                var entries = await ProcessSingleTransaction(command, HasPaidCommissionByCash);
                allEntries.AddRange(entries);

                // Add entries to the repository for saving
                _accountingEntryRepository.AddRange(entries);
            }

            return allEntries;
        }

        // Process a single transaction to generate accounting entries
        private async Task<List<Data.AccountingEntry>> ProcessSingleTransaction(MakeAccountPostingCommand command, bool HasPaidCommissionByCash)
        {
            // Initialize the required accounts for the transaction
            var accounts = await InitializeAccounts(command);
            var entriesDto = new List<Data.AccountingEntryDto>();

            // Process the main transaction
            entriesDto.AddRange(await ProcessMainTransaction(command, accounts));

            // Process commissions and special fees
            entriesDto.AddRange(await ProcessCommissions(command, accounts));
            entriesDto.AddRange(await ProcessSpecialFees(command, accounts, HasPaidCommissionByCash));

            // Map DTOs to accounting entries
            var entries = _mapper.Map(entriesDto, new List<Data.AccountingEntry>());

            return entries;
        }
        /// <summary>
        /// Processes special event fees
        /// </summary>
        /// <param name="command"></param>
        /// <param name="accounts"></param>
        /// <returns></returns>
        private async Task<List<AccountingEntryDto>> ProcessSpecialFees(MakeAccountPostingCommand command, TransactionAccounts accounts, bool HasPaidCommissionByCash)
        {
            var entries = new List<AccountingEntryDto>();

            foreach (var fee in command.AmountEventCollections)
            {
                var eventAccount = await _accountingService.GetAccountByEventCode(fee, _userInfoToken.BranchId, _userInfoToken.BranchCode);
                var sourceAccount = HasPaidCommissionByCash ? accounts.TellerAccount : accounts.ProductAccount;

                entries.AddRange(await _accountingService.CashMovementAsync(fee.Naration, command.MemberReference,
                command.TransactionDate,
                sourceAccount,
                eventAccount,
                fee.Amount,
                "MakeAccountPostingCommand",
                command.TransactionReferenceId, _userInfoToken.BranchId));
            }

            return entries;
        }
 
        // Process Commision Account
        private async Task<List<AccountingEntryDto>> ProcessCommissions(MakeAccountPostingCommand command, TransactionAccounts accounts)
        {
            var entries = new List<AccountingEntryDto>();
            var principalEvent = command.AmountCollection.FirstOrDefault(x => x.IsPrincipal);

            if (!command.AmountCollection.Remove(principalEvent)) return entries;
            var collection = command.AmountCollection.Where(x => x.Amount > 0);
            foreach (var amount in collection)
            {
                var (branchId, branchCode) = GetBranchInfo(command, amount);
                var commissionAccount = await _accountingService.GetCommissionAccount(GetOperationType( command.OperationType),
                    amount, command.ProductId, branchId, branchCode, command.IsRemittance());

                var sourceAccount = amount.HasPaidCommissionByCash == true
                    ? accounts.TellerAccount
                    : accounts.ProductAccount;

                entries.AddRange(await _accountingService.CashMovementAsync(amount.Naration, command.MemberReference,
                command.TransactionDate,
                sourceAccount,
                commissionAccount,
                amount.Amount,
                "MakeAccountPostingCommand",
                command.TransactionReferenceId, _userInfoToken.BranchId));
            }

            return entries;
        }
        public OperationEventAttributeTypes GetOperationType(string operation)
        {
            if (OperationEventAttributeTypes.deposit.ToString().Equals(operation.ToLower()))
            {
                return OperationEventAttributeTypes.deposit;
            }
            else
            {
                return OperationEventAttributeTypes.withdrawal;
            }
        }
     
        // get the branch information to be used
        private (string branchId, string branchCode) GetBranchInfo(MakeAccountPostingCommand command, AmountCollection amount) =>
         amount.CheckCommissionAccountType()
             ? (command.ExternalBranchId, command.ExternalBranchCode)
             : (_userInfoToken.BranchId, _userInfoToken.BranchCode);

        private string GetExternalBranchId(MakeAccountPostingCommand command) =>
            command.IsInterBranchTransaction ? command.ExternalBranchId : "NOT-SET";



        // Initialize the required accounts for a transaction
        private async Task<TransactionAccounts> InitializeAccounts(MakeAccountPostingCommand command)
        {
            var tellerAccount = await _accountingService.GetTellerAccount(command);
            var productEvent = command.AmountCollection.FirstOrDefault(x => x.IsPrincipal);
            var productAccount = await GetProductAccount(command, productEvent);
            var HomeliaisonAwayAccount = command.IsInterBranchTransaction
                ? await _accountingService.GetHomeliaisonAwayAccount(command)
                : null;
            var AwayliaisonAccount = command.IsInterBranchTransaction
             ? await _accountingService.GetAwayLiaisonAccountHome(command)
             : null;

            return new TransactionAccounts(tellerAccount, productAccount, HomeliaisonAwayAccount, AwayliaisonAccount);
        }

        // Get the product account for a specific transaction
        private async Task<Data.Account> GetProductAccount(MakeAccountPostingCommand command, AmountCollection productEvent)
        {
            var (branchId, branchCode) = command.IsInterBranchTransaction
                ? (command.ExternalBranchId, command.ExternalBranchCode)
                : (_userInfoToken.BranchId, _userInfoToken.BranchCode);

            return await _accountingService.GetProductAccount(productEvent, command.ProductId, branchId, branchCode);
        }

        // Process the main transaction entries
        private async Task<List<AccountingEntryDto>> ProcessMainTransaction(MakeAccountPostingCommand command, TransactionAccounts accounts)
        {
            var isDeposit = command.OperationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString());

            return command.IsInterBranchTransaction
                ? await ProcessInterBranchTransaction(command, accounts, isDeposit)
                : await ProcessLocalTransaction(command, accounts, isDeposit);
        }

        // Validate and save accounting entries using double-entry rules
        private async Task<bool> ValidateAndSaveEntries(List<Data.AccountingEntry> entries)
        {
            if (!_accountingService.EvaluateDoubleEntryRule(entries))
                return false;

         await  _uow.SaveAsyncWithOutAffectingBranchId();
            return true;
        }
        // process local transaction 
        private async Task<List<AccountingEntryDto>> ProcessLocalTransaction(MakeAccountPostingCommand command, TransactionAccounts accounts, bool isDeposit)
        {
            var ProductEventCode = command.AmountCollection.Where(x => x.IsPrincipal).FirstOrDefault();
            return isDeposit
                ? await _accountingService.CashMovementAsync(ProductEventCode.Naration ,command.MemberReference,
                    command.TransactionDate, accounts.TellerAccount, accounts.ProductAccount,
                    command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId)
                : await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference,
                    command.TransactionDate, accounts.ProductAccount, accounts.TellerAccount,
                    command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId);
        }
        // process Inter Branch Transaction
        private async Task<List<AccountingEntryDto>> ProcessInterBranchTransaction(MakeAccountPostingCommand command, TransactionAccounts accounts, bool isDeposit)
        {
            var entries = new List<AccountingEntryDto>();

            var ProductEventCode = command.AmountCollection.Where(x => x.IsPrincipal).FirstOrDefault();
            if (isDeposit)
            {
                entries.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference,
                    command.TransactionDate, accounts.TellerAccount, accounts.AwayLiaisonAccount,
                    command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, false,command.ExternalBranchId));

                entries.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference,
                    command.TransactionDate, accounts.HomeLiaisonAccountAway, accounts.ProductAccount,
                    command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, command.IsInterBranchTransaction, command.ExternalBranchId));
            }
            else
            {
                entries.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference,
                    command.TransactionDate, accounts.HomeLiaisonAccountAway, accounts.TellerAccount,
                    command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, command.IsInterBranchTransaction, command.ExternalBranchId));

                entries.AddRange(await _accountingService.CashMovementAsync(ProductEventCode.Naration, command.MemberReference,
                    command.TransactionDate, accounts.ProductAccount, accounts.AwayLiaisonAccount,
                    command.GetPrincipalAmount(), "MakeAccountPostingCommand", command.TransactionReferenceId, _userInfoToken.BranchId, false, command.ExternalBranchId));
            }

            return entries;
        }

        #endregion

        #region Nested Types
        // Record structure to group transaction accounts
        public record TransactionAccounts(Data.Account TellerAccount, Data.Account ProductAccount, Data.Account HomeLiaisonAccountAway, Data.Account AwayLiaisonAccount);
        #endregion

        // Additional processing methods omitted for brevity...
        #endregion
    }
    #endregion
}
