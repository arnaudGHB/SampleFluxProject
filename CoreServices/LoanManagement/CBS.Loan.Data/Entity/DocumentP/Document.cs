using CBS.NLoan.Data.Entity.LoanApplicationP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.DocumentP
{
    public class Document : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<DocumentJoin> DocumentJoins { get; set; }
        public virtual ICollection<LoanApplication> LoanApplication { get; set; }
    }
}
