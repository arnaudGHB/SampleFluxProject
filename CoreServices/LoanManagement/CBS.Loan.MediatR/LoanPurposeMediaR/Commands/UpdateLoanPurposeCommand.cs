using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanPurposeCommand : IRequest<ServiceResponse<LoanPurposeDto>>
    {
        public string Id { get; set; }
        public string PurposeName { get; set; }
        public string LoanProductCategoryId { get; set; }

    }

}
