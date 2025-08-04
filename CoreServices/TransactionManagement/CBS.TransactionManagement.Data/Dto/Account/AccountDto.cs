using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using CBS.TransactionManagement.Helper;
using System.Collections.ObjectModel;

namespace CBS.TransactionManagement.Dto
{
    public class AccountDto
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; } = 0;
        public decimal PreviousBalance { get; set; } = 0;
        public string Status { get; set; } = AccountStatus.Inprogress.ToString();
        public string ProductId { get; set; }
        public string? CustomerId { get; set; }
        public string? TellerId { get; set; }
        public string? BranchCode { get; set; }
        public string CustomerName { get; set; }
        public string? EncryptedBalance { get; set; }
        public decimal InterestGenerated { get; set; } = 0;
        public decimal LastInterestPosted { get; set; } = 0;
        public decimal BlockedAmount { get; set; } = 0;
        public string? BlockedId { get; set; }
        public DateTime DateBlocked { get; set; } = DateTime.MinValue;
        public DateTime DateReleased { get; set; } = DateTime.MinValue;
        public string? ReasonOfBlocked { get; set; }
        //public decimal TellerInterestBalance { get; set; }
        public DateTime? DateOfLastOperation { get; set; } = DateTime.MinValue;
        public DateTime? LastInterestCalculatedDate { get; set; } = DateTime.MinValue;
        public string? AccountName { get; set; }
        public string? LastOperation { get; set; }
        public string AccountType { get; set; }
        public bool IsTellerAccount { get; set; }
        public string? OpenningOfDayStatus { get; set; } = CloseOfDayStatus.CLOSED.ToString();
        public DateTime? OpenningOfDayDate { get; set; } = DateTime.MinValue;
        public string? OpenningOfDayReference { get; set; }
        public decimal OpeningBalance { get; set; }
        public DateTime? DateOfOpeningBalance { get; set; } = DateTime.MinValue;
        public decimal? LastOperationAmount { get; set; } = 0;
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public SavingProduct Product { get; set; }
        public List<WithdrawalNotification> WithdrawalNotifications { get; set; }

    }
    public class AccountStatisticsDto
    {
        public int TotalNumberOfAccounts { get; set; }
        public int TotalBranches { get; set; }
        public int TotalMembers { get; set; }
        public int TotalNumberOfActiveAccounts { get; set; }
        public int TotalNumberOfInActiveAccounts { get; set; }
        public string BranchId { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal TotalBalanceWithoutBlocked { get; set; }
        public decimal TotalBlockedAmount { get; set; }
        public List<StatisticPerAccountDto> StatisticPerAccounts { get; set; }
    }
    public class StatisticPerAccountDto
    {
        public int NumberOfAccounts { get; set; }
        public int NumberOfActiveAccounts { get; set; }
        public int NumberOfInActiveAccounts { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public decimal BlockedAmount { get; set; }
        public decimal BalanceWithoutBlocked { get; set; }
    }

    public class DashboardStatisticsDto
    {
        public string BranchName { get; set; }
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string DashboardAccountType { get; set; }
        public decimal Balance { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
    }
    enum DashboardAccountType
    {
        CashInHand, CashInBank, TotalShares, PreferenceShare, OrdinaryShares, Deposit, Savings, Gav, DailyCollections, MTNMobileMoney, OrangeMoney, TotalExpense, TotalIncome, TotalLiquidity
    }
}