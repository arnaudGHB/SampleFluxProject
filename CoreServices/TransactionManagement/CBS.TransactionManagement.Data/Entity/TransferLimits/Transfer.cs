using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.TransferLimits
{
    public class Transfer: BaseEntity
    {
        public string Id { get; set; }
        public string SourceAccountNumber { get; set; }
        public string DestinationAccountNumber { get; set; }
        public string SourceAccountType { get; set; }
        public string DestinationAccountType { get; set; }
        public decimal Charges { get; set; }
        public decimal Tax { get; set; }
        public string TransactionRef { get; set; }
        public string TransactionType { get; set; }
        public decimal SourceCommision { get; set; }
        public decimal DestinationCommision { get; set; }
        public DateTime AccountingDate { get; set; }
        public bool IsInterBranchOperation { get; set; }
        public decimal Amount { get; set; }
        public string SourceType { get; set; }
        public string Status { get; set; }
        public string? ApprovedByUserName { get; set; }
        public string InitiatedByUSerName { get; set; }
        public DateTime DateOfInitiation { get; set; } = DateTime.Now;
        public DateTime DateOfApproval { get; set; } = DateTime.MinValue;
        public string InitiatorComment { get; set; }
        public string? ValidatorComment { get; set; }
        public string BranchId { get; set; }
        public string? SourceAccountName { get; set; }
        public string? DestinationAccountName { get; set; }
        public string? SenderName { get; set; }
        public string? RecieverName { get; set; }

        public string? SourceBranchName { get; set; }
        public string? DestinationBranchName { get; set; }

        public string AccountId { get; set; }
        public string TellerId { get; set; }
        public virtual Account Account { get; set; }
        public virtual Teller Teller { get; set; }
        public string SourceBrachId { get; set; }
        public string DestinationBrachId { get; set; }
    }
}
