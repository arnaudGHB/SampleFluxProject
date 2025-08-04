using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.CustomerLoanAccountP
{
    public class CustomerLoanAccount : BaseEntity
    {

        [Key]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string Balance { get; set; }
        public string PreviousBalance { get; set; }
        public string LastLoanId { get; set; }
        public string EncryptionCode { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        //public virtual ICollection<LoanProductCustomerProfileJoin> LoanProductCustomerProfileJoins { get; set; }

    }
}
