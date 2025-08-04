using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Data.Dto.CollateraP
{
    public class LoanApplicationCollateralDto
    {
        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public string LoanProductCollateralId { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? Reference { get; set; }
        public virtual LoanProductCollateral LoanProductCollateral { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }

    }
}

