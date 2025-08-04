using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.LoanRepayment
{
    public class TellerLoanPaymentObject
    {
        public string TransactionReference { get; set; }
        public string EvenetType { get; set; }
        public decimal CurrentBalance { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal DestinationBranchCommission { get; set; }
        public decimal SourceBranchCommission { get; set; }
        public string DestinationBrachId { get; set; }
        public string SourceBranchId { get; set; }
        public string LocalAccountNumber { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public decimal Amount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Vat { get; set; }
        public string OperationType { get; set; }
        public bool IsInterBranch { get; set; }
        public string LoanRepaymentType { get; set; }
        public DateTime AccountingDate { get; set; }
    }

}
