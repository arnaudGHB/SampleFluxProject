using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.RefundP;

namespace CBS.NLoan.Data.Dto.RefundP
{
    public class RefundDto
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
        public DateTime DateOfPayment { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }
        public virtual Loan Loan { get; set; }
        public bool IsComplete { get; set; }
        public virtual ICollection<RefundDetail> RefundDetails { get; set; }
    }
    public class PaymentStatus
    {
        public int AdvancedPaymentDays { get; set; }
        public int DelinquentDays { get; set; }
        public decimal AdvancedPaymentAmount { get; set; }
        public decimal DelinquentAmount { get; set; }
        public DateTime NextPaymentDate { get; set; }
    }

}
