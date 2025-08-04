using CBS.NLoan.Data.Entity.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.FeeP
{
    public class LoanApplicationFee : BaseEntity
    {
        public string Id { get; set; }
        public string FeeRangeId { get; set; }
        public decimal FeeAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string? FeeLable { get; set; }
        public string? EventCode { get; set; }
        public bool IsPaid { get; set; }
        public string CustomerId { get; set; }
        public bool IsCashDeskPayment { get; set; }
        public string Status { get; set; }
        public string? Period { get; set; }
        public string? TransactionReference { get; set; }
        public string LoanApplicationId { get; set; }
        public virtual FeeRange FeeRange { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }
        public DateTime DateOfPayment { get; set; } = DateTime.MinValue;                   
        public string? PaidBy { get; set; }

    }
}
