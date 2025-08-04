using CBS.NLoan.Data.Entity.LoanPurposeP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanProductCategory : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<LoanProduct> LoanProducts { get; set; }
        public virtual ICollection<LoanPurpose> LoanPurposes { get; set; }
        
    }
}
