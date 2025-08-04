using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanCommentry : BaseEntity
    {

        [Key]
        public string Id { get; set; }
        public string LoanId { get; set; }
        public string Comment { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public virtual Loan Loan { get; set; }

    }
}
