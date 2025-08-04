using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Data.Dto.TaxP
{
    public class TaxDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal TaxRate { get; set; }
        public bool AppliedWhenLoanRequestIsGreaterThanSaving { get; set; }
        public bool AppliedOnInterest { get; set; }
        public bool IsVat { get; set; }
        public decimal SavingControlAmount { get; set; }
        public string Description { get; set; }
        public List<LoanApplication> LoanApplications { get; set; }
    }
}
