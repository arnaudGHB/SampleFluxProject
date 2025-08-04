using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class RescheduleLoan : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string LoanId { get; set; }
        public virtual Loan Loan { get; set; }
    }
}
