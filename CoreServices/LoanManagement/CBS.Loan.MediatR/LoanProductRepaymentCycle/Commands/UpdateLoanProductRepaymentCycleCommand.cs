using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanProductRepaymentCycleCommand : IRequest<ServiceResponse<LoanProductRepaymentCycleDto>>
    {
        public string Id { get; set; }
        public int RepaymentOrder { get; set; }
        public string RepaymentReceive { get; set; }
        public string LoanProductId { get; set; }
    }

}
