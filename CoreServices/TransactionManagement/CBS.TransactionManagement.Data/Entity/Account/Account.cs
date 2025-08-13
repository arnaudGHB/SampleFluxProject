using CBS.TransactionManagement.Data.Dto.Resource;
using CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP;
using CBS.TransactionManagement.Helper;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.TransactionManagement.Data
{
    public class Account : BaseEntity
    {
        [Key]
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
        public virtual SavingProduct Product { get; set; }
        public virtual ICollection<WithdrawalNotification> WithdrawalNotifications { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
    public class MembersAccountSummaryDto
    {
        public string MemberName { get; set; }
        public string MemberReference { get; set; }
        public string BranchCode { get; set; }
        public decimal Saving { get; set; }
        public decimal PreferenceShare { get; set; }
        public decimal Share { get; set; }
        public decimal Deposit { get; set; }
        public decimal Loan { get; set; }
        public decimal Gav { get; set; }
        public decimal DailyCollection { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal NetBalance { get; set; }
        public PaginationMetadata PaginationMetadata { get; set; }

    }
   
   
}