using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class TellerOperationDto
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string AccountID { get; set; }
        public string? AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public string TransactionRef { get; set; }
        public string TransactionID { get; set; }
        public decimal PreviousBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public string TellerID { get; set; }
        public string? UserID { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? EntryDate { get; set; }

        public string BankId { get; set; }
        public string BranchId { get; set; }
        public TransactionDto Transaction { get; set; }

    }
    public class TellerOperationGL
    {
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public string TransactionRef { get; set; }
        public string DailyReferences { get; set; }
        public decimal BalanceBF { get; set; }
        public string MemberId { get; set; }
        public string Description { get; set; }
        public string MemberAccountNumber { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Naration { get; set; }
        public string MemberName { get; set; }
        public decimal Balance { get; set; }
        public string TellerID { get; set; }
        public DateTime Date { get; set; }
        public DateTime? EntryDate { get; set; }

        public string BranchId { get; set; }

    }
}

