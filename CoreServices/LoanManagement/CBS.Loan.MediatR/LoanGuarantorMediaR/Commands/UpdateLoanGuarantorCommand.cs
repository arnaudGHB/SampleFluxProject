using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanGuarantorCommand : IRequest<ServiceResponse<LoanGuarantorDto>>
    {
        public string Id { get; set; }
        public string LoanApplicationId { get; set; }
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

    }

}
