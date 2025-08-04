using CBS.AccountManagement.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountingEntry : BaseEntity
    {
        [Key]
        // Unique ID number for the entry=
        public string Id { get; set; }

        // Date the accounting entry was created
        public DateTime EntryDate { get; set; }
        // Effective date for posting the accounting impact
        public DateTime ValueDate { get; set; }
        // Type of entry (Debit or Credit)
        public string EntryType { get; set; }
        // Currency denomination 
        public string Currency { get; set; }
        // Text description explaining the purpose of the transaction
        public string Representative { get; set; } = "XXXX";
        public string Naration { get; set; }
        // ID linking to source documents related to transaction
        public string ReferenceID { get; set; }
        // Status of workflow ( Posted,  Reversed)
        public string Status { get; set; }
        // Source system or module that generated the entry
        public string Source { get; set; }
        public string BankId { get; set; } // Related branch 
        public string BranchId { get; set; } // Related branch 
        public bool IsAuxilaryEntry { get; set; } = false;
        public string? DrAccountNumber { get; set; }
        public string? DrAccountId { get; set; }
        public string? CrAccountId { get; set; }
        public string? AccountId { get; set; }
        public string? CrAccountNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal DrAmount { get; set; }
        public decimal CrAmount { get; set; }
        public decimal CurrentBalance { get; set; } = 0;
        public decimal DrBalanceBroughtForward{ get; set; } = 0;
        public decimal CrBalanceBroughtForward { get; set; } = 0;
        public decimal CrCurrentBalance { get; set; }
        public decimal DrCurrentBalance { get; set; }
        public string ExternalBranchId { get; set; }
        public string AccountCartegory { get; set; }
        public string InitiatorId { get; set; }
        public string EventCode { get; set; }
        public string OperationType { get; set; }
        public string AccountNumber { get; set; }
        public string? AccountNumberReference { get; set; }
        public void TagedAsReversed(List<AccountingEntry> entries, string userId)
        {
            foreach (var entry in entries)
                Status = PostingStatus.Reversed;
        }
        public void TagedAsPosted(List<AccountingEntry> entries)
        {
            foreach (var entry in entries)
                Status = PostingStatus.Posted;
        }
    }


}
