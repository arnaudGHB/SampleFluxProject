using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.APICaller.Helper.LoginModel.Authenthication;

namespace CBS.AccountManagement.MediatR
{
    public interface IAccountingEntriesServices
    {
        Task<bool> TransactionExists(string transactionReferenceId);
        Data.Account? GetAccountByAccountNumber(string accountNumber, string branchId);
        Task<(bool, string)> CheckDuplicateEntries(string errorMessage, string referenceId);
        Task<bool> ChackIfOperationIsValid(bool IsDeposite, Data.Account account, decimal amount, AccountOperationType operationType);
        Task<Data.Account> UpdateAccountAsync(Data.Account account, decimal amount, AccountOperationType operationType);
        Task<Data.Account> GetAccount(string EventCode, string BranchId, string BranchCode, bool remittanceStatus = false);
         Task CreateAccountByChartOfAccountManagementPositionId(string chartOfAccountManagementPositionId, string branchCode, string BranchId);
        Task<Data.Account> CreateAccountForBranchByChartOfAccountIdAsync(string determinationAccountId, string branchId, string branchCode);
        AccountingEntryDto CreateCreditEntry(Booking book, Data.Account CrAccount, Data.Account DrAccount,DateTime TransactionDate);
        AccountingEntryDto CreateDebitEntry(Booking book, Data.Account CrAccount, Data.Account DrAccount, DateTime TransactionDate);
        Data.AccountingEntry CreateEntry(Data.Account account, EntryTempData item, string InitiatorId, DateTime TransactionDate);
        bool EvaluateDoubleEntryRule(List<Data.AccountingEntry> listResult);
        TransactionData GenerateTransactionRecord(string accountNumber, string types, string Code, string transactionReferenceId, string naration, decimal amount);
        TransactionData GenerateTransactionRecord(string accountNumber, OperationEventAttributeTypes types, TransactionCode Code, string transactionReferenceId, string naration, decimal amount);
        //Task<Data.Account> GetAccount(string EventCode, string BranchId, string BranchCodeX);
        Task<Data.Account> GetMFI_ChartOfAccount(string MFI_ChartOfAccountId, string BranchId, string BranchCode);
        Task<Data.Account> GetAccountBasedOnChartOfAccountID(string? balancingAccountId, string BranchId, string BranchCode);
        Task<Data.Account> GetAccountByEventCode(Commands.AmountEventCollection command, string branchID, string BranchCode);
        Task<Data.Account> GetAccountByEventCode(Data.BulkTransaction.AmountEventCollection command, string branchID, string BranchCode);
        AccountingRuleEntry GetAccountEntryRuleByEventCode(string? eventCode);
        AccountingRuleEntry GetAccountEntryRuleByProductID(string? eventCode);
        Task<(Data.Account DeterminantAccount, Data.Account BalancingAccount)> GetCashMovementAccountByEventCode(string eventCode, string branchID, string branchCode);
        Task<(Data.Account DeterminantAccount, Data.Account BalancingAccount, AccountOperationType BookingDirection)> GetCashMovementAccountWithBookingDirectionByEventCode(string eventCode, string branchID, string branchCode);
        Task<Data.Account> GetAccountForProcessing(string ChartOfAccountId, AddTransferEventCommand command);
        Task<Data.Account> GetAccountForProcessing(string ChartOfAccountId, MakeNonCashAccountAdjustmentCommand command);
        List<AccountingRuleEntry>? GetAccountingEntryRule(string? eventCode);
        AccountingRuleEntry GetAccountingEntryRuleByEventCode(string OpeningEvent);
        //ChartOfAccountManagementPosition GetAccountNumberManagementPosition(Data.ChartOfAccount chartOfAccount);
        Task<Branch> GetBranchCodeAsync(string externalBranchId);
        Task<Data.Account> GetCommissionAccount(AmountCollection? productEventCode, string productName, string BranchId, string branchCode, bool IsRemittance = false);
        Task<Data.Account> GetTransferCommissionAccount( Data.BulkTransaction.AmountCollection? productEventCode, string productName, string BranchId, string branchCode, bool IsRemittance = false);

        Task<Data.Account> GetCommissionAccount(OperationEventAttributeTypes operationType, AmountCollection? productEventCode, string productName, string BranchId, string branchCode, bool IsRemittance = false);
        Task<Data.Account> GetCommissionAccount(AddTransferEventCommand.TransferAmountCollection productEventCode, string productName, string BranchId, string branchCode);
        Task<Data.Account> GetLiaisonAccount(AddTransferEventCommand command);
        Task<Data.Account> GetHomeLiaisonAccount(MakeAccountPostingCommand command);
        Task<List<AccountingEntryDto>> TransferFundBetweenBranch(string naration, string memberReference, DateTime TransactionDate, Data.Account fromAccount, Data.Account toAccount, Data.Account liaisonAccount, decimal amount, AddTransferEventCommand eventCommand);
        //  Task<List<AccountingEntryDto>> TransferFunds(string naration, string memberReference, DateTime TransactionDate, Data.Account fromAccount, Data.Account toAccount, decimal amount, AddTransferEventCommand eventCommand);
        Task<Data.Account> GetAwayLiaisonAccountHome(MakeAccountPostingCommand command);

        Task<Data.Account> GetAwayLiaisonAccountHome(Data.BulkTransaction.MakeAccountPostingCommand command);

        Task<List<CashMovementAccount>> GetListAccountAndOperationTypesAsync(List<AccountingRuleEntry> ruleEntries, string EventCode, string BranchId, string BranchCode);
        Task<Data.Account> GetProductAccount(AmountCollection? productEventCode, string productName, string BranchId, string branchCode);
        Task<Data.Account> GetProductAccount(Data.BulkTransaction.AmountCollection? productEventCode, string productName, string BranchId, string branchCode);

        Task<Data.Account> GetProductAccount(CollectionAmount? productEventCode, string productName, string BranchId, string branchCode);

        Task<Data.Account> GetTellerAccount(MakeAccountPostingCommand command);


        Task<Data.Account> GetTellerAccount(MakeRemittanceCommand command);

        Task<Data.Account> GetTellerAccount(Data.BulkTransaction.MakeAccountPostingCommand command);

        // Task<List<AccountingEntryDto>> UpdateAccountBalanceAsync(string naration, DateTime Transdate, Data.Account debitAccount, Data.Account creditAccount, decimal Amount, string TransactionReferenceId, bool IsInterBranchTransaction = false, string externalBranchId = "not-set");
        Task<Data.Account> UpdateAccountBalanceAsync(string naration, Data.Account Account, decimal amount, AccountOperationType operationType, string PostingEvent);
        Task<List<AccountingEntryDto>> CashMovementAsync(string naration, string memberReference, DateTime TransactionDate, Data.Account debitAccount, Data.Account creditAccount, decimal Amount, string PostingEventCommand, string TransactionReferenceId, string branchId, bool IsInterBranchTransaction = false, string externalBranchId = "not-set");
     
        Task<Data.Account?> GetAccountByAccountNumberInsteadHavingId(string accountNumber, string branchId, string branchCode);
        Task<Data.Account?> GetAccountUsingMFIChart(string? ChartOfAccountId, string toBranchId, string toBranchCode);
        Task<Data.Account> GetAccountUsingMFIChartForliason(string? ChartOfAccountId, string toBranchId, string toBranchCode);


        Task<Data.Account> GetHomeliaisonAwayAccount(Data.BulkTransaction.MakeAccountPostingCommand command);

        Task<Data.Account> GetHomeliaisonAwayAccount(MakeAccountPostingCommand command);
        Task<Data.Account> CreateLiaisonAccountForAwayBranchWithChartOfAccountIdAsync(string determinationAccountId, string LiasonBranchId, string LiasonBranchBranchCode, string OwnerBranchId, string OwnerbranchCode);
         Task<bool>  CheckIfDoubbleVaidationIsRequired(string systemId);
        Task<bool> IsBranchEligibility(string systemId, string branchId);
        Task<Data.Account> GetAccountByProductID(string fromProductId, string productType, string branchId, string branchCode);
}
}