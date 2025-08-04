using CBS.NLoan.Data.Entity.LoanApplicationP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.LoanPurposeP
{
    public class LoanPurpose : BaseEntity
    {
       
        public string Id { get; set; }
        public string PurposeName { get; set; }
        public string? LoanProductCategoryId { get; set; }

        public virtual LoanProductCategory LoanProductCategory { get; set; }
    }
}
