using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanGuarantor : BaseEntity
    {

       
        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
        public string GuarantorType { get; set; }

        public string GuarantorName { get; set; }
        public string? CustomerId { get; set; }
        public string IdCardNumber { get; set; }
        public string ExpireDate { get; set; }
        public string IssueDate { get; set; }
        public string? Relationship { get; set; }
        public string? Address { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsCoMember { get; set; }
        public string AccountNumber { get; set; }
        public string? Email { get; set; }
        public decimal GuaranteeAmount { get; set; }
        public virtual LoanApplication LoanApplication { get; set; }

    }
}
