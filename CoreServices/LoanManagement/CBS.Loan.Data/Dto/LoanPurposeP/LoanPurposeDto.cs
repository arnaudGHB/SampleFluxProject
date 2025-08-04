using CBS.NLoan.Data.Entity.LoanApplicationP;

namespace CBS.NLoan.Data.Dto.LoanPurposeP
{
    public class LoanPurposeDto
    {
        public string Id { get; set; }
        public string PurposeName { get; set; }
        public string LoanProductCategoryId { get; set; }

        public LoanProductCategory LoanProductCategory { get; set; }
    }
}
