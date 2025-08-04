using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanCommand : IRequest<ServiceResponse<LoanDto>>
    {
        public string LoanApplicationId { get; set; }
        public string CustomerName { get; set; }
        public string BranchCode { get; set; }
    }

}
