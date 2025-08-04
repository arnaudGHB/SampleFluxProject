using CBS.NLoan.Data.Entity.LoanApplicationP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.RefundP
{
    public class Refund : BaseEntity
    {

        public string Id { get; set; }
        public string LoanId { get; set; }
        public string CustomerId { get; set; }
        public string Comment { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentChannel { get; set; }
        public decimal Amount { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public decimal Penalty { get; set; }
        public decimal Tax { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public decimal Balance { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime DateOfPayment { get; set; }
        public virtual Loan Loan { get; set; }
        public virtual ICollection<RefundDetail> RefundDetails { get; set; }
        
    }
}
