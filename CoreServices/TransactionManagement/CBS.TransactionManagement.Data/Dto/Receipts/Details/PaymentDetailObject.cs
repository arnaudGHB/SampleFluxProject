using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.Receipts.Details
{
    public class PaymentDetailObject
    {
        public string SericeOrEventName { get; set; }
        public decimal Amount { get; set; } = 0;
        public decimal Fee { get; set; } = 0;
        public decimal LoanCapital { get; set; } = 0;
        public decimal Interest { get; set; } = 0;
        public decimal VAT { get; set; } = 0;
        public decimal Balance { get; set; }
        public string? AccountNumber { get; set; }
    }
}
