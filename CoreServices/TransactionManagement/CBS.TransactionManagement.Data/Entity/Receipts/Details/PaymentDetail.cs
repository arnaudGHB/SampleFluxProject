using CBS.TransactionManagement.Data.Entity.Receipts.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.Receipts.Details
{
   
    public class PaymentDetail : BaseEntity
    {
        public string Id { get; set; }
        public string MemberName { get; set; }
        public string MemberReference { get; set; }
        public string PaymentReceiptId { get; set; }
        public string? SericeName { get; set; }
        public decimal Amount { get; set; } = 0;
        public decimal Fee { get; set; } = 0;
        public decimal LoanCapital { get; set; } = 0;
        public decimal Interest { get; set; } = 0;
        public decimal VAT { get; set; } = 0;
        public string? AccountNumber { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public DateTime AccountingDay { get; set; }
        public DateTime Date { get; set; }
        public virtual PaymentReceipt PaymentReceipt { get; set; }
    }
   
}
