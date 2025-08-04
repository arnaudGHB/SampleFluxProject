using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.WithdrawalNotificationP
{
    public class WithdrawalNotification: BaseEntity
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountId { get; set; }

        public DateTime NotificationDate { get; set; } = DateTime.MinValue;
        public DateTime DateOfIntendedWithdrawal { get; set; } = DateTime.MinValue;
        public DateTime GracePeriodDate { get; set; } = DateTime.MinValue;
        public decimal AmountRequired { get; set; }
        public string ReasonForWithdrawal { get; set; }//LoanRepayment Or Consumption, Others
        public decimal AccountBalance { get; set; }
        public decimal LoanBalance { get; set; }
        public string? Purpose { get; set; }
        public bool IsNotificationPaid { get; set; }
        public bool IsExpired { get; set; }
        public decimal FormNotificationCharge { get; set; }
        public bool IsWithdrawalDone { get; set; }
        public string? ApprovalStatus { get; set; }
        public DateTime? ApprovalDate { get; set; } = DateTime.MinValue;
        public string? ApprovedByName { get; set; }
        public string? ApprovedComment { get; set; }
        public string? TransactionReference { get; set; }
        public DateTime DateWithdrawalWasDone { get; set; } = DateTime.MinValue;
        public DateTime DateFormFeeWasPaid { get; set; } = DateTime.MinValue;
        public string? TellerId { get; set; }
        public string? TellerName { get; set; }
        public string? TellerCaise { get; set; }
        public string? TellerId_fee { get; set; }
        public string? TellerName_fee { get; set; }
        public string? TellerCaise_fee { get; set; }
        public string? ServiceClearName { get; set; }
        public string? InitiatingBranchId { get; set; }
        public string? MemberBranchId { get; set; }

        public virtual Account Account { get; set; }

    }
}
