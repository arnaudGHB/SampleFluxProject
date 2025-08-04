using CBS.NLoan.Data.Entity.RefundP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanAmortization:BaseEntity
    {
        public string Id { get; set; }
        public string LoanId { get; set; }
        public decimal Amount { get; set; }//The loan amount that is submited for simulation
        public int Sno { get; set; }
        public decimal BegginingBalance { get; set; }
        public decimal Principal { get; set; }
        public decimal PrincipalBalance { get; set; }
        public decimal PrincipalPaid { get; set; }
        public decimal Interest { get; set; }
        public decimal InterestBalance { get; set; }
        public decimal PreviouseInterest { get; set; }
        public decimal InterestPaid { get; set; }
        public decimal InterestDue { get; set; }
        public decimal TotalDue { get; set; }
        public decimal Balance { get; set; }
        public decimal Fee { get; set; }
        public decimal Annuity { get; set; }
        public string Description { get; set; }
        public decimal Penalty { get; set; }
        public decimal PenaltyPaid { get; set; }
        public decimal Tax { get; set; }
        public bool PreviousInstallmentDue { get; set; }
        public DateTime LastServiceCalculationDate { get; set; } = DateTime.MinValue;
        public decimal TaxPaid { get; set; }
        public decimal Paid { get; set; }
        public decimal Due { get; set; }
        public DateTime DateOfPayment { get; set; }
        public string Status { get; set; }
        public string MethodOfPayment { get; set; }
        public bool IsStructured { get; set; }
        public string? LoanStructuringStatus { get; set; }
        public bool IsCompleted { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public DateTime NextPaymentDate { get; set; }
        public Loan Loan { get; set; }
        public List<RefundDetail> RefundDetails { get; set; }
    }
   
}
