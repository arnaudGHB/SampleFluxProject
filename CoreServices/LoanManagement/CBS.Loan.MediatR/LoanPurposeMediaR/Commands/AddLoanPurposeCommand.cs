using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanPurposeCommand : IRequest<ServiceResponse<LoanPurposeDto>>
    {
        public string LoanProductCategoryId { get; set; }
        public string PurposeName { get; set; }
    }

}
