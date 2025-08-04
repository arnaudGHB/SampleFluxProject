using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateCustomerLoanAccountCommand : IRequest<ServiceResponse<CustomerLoanAccountDto>>
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string Balance { get; set; }
        public string PreviousBalance { get; set; }
        public string LastLoanId { get; set; }
        public string EncryptionCode { get; set; }
        public string OrganizationId { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
    }

}
