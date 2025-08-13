using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class TellerOperation:BaseEntity
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public decimal SourceBranchCommission { get; set; }
        public decimal DestinationBranchCommission { get; set; }
        public string AccountID { get; set; }
        public string OperationType { get; set; }
        public string? AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public string EventName { get; set; }
        public bool IsInterBranch { get; set; }
        public bool IsCashOperation { get; set; } = true;
        public string? TransactionReference { get; set; }
        public string? SourceBranchId { get; set; }
        public string? DestinationBrachId { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public string? ReferenceId { get; set; }
        public string? CustomerId { get; set; }
        public string? MemberName { get; set; }
        public string? MemberAccountNumber { get; set; }
        public string TellerID { get; set; }
        public string? UserID { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? AccountingDate { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string Description { get; set; }
        public string OpenOfDayReference { get; set; }
        public TellerOperation()
        {
            IsCashOperation = true;
        }

    }
}

