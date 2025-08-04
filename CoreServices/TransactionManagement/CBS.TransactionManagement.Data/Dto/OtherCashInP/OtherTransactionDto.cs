using CBS.TransactionManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.OtherCashInP
{
    public class OtherTransactionDto
    {
        public string Id { get; set; }
        public string TransactionReference { get; set; }
        public string? EnventName { get; set; }
        public string? Description { get; set; }
        public string TellerId { get; set; }
        public decimal Amount { get; set; }
        public string EventCode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? AccountNumber { get; set; } = "N/A";
        public string? Direction { get; set; }
        public string? TransactionType { get; set; }//Income Or Expenses
        public string? SourceType { get; set; }//Cash_Collection Or Member_Account
        public string? MemberName { get; set; }
        public string? CNI { get; set; }
        public string? TelephoneNumber { get; set; }
        public string? CustomerId { get; set; }
        public DateTime DateOfOperation { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string AmountInWord { get; set; }
        public string ReceiptTitle { get; set; }
        public virtual Teller Teller { get; set; }
        public string Narration { get; set; }
    }
}
