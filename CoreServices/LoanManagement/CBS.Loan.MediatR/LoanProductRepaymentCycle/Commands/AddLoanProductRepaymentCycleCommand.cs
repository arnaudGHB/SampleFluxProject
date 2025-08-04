using CBS.NLoan.Data;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Data.Entity.TaxP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanProductRepaymentCycleCommand : IRequest<ServiceResponse<bool>>
    {
        public List<string> RepaymentCycles { get; set; }
        public string LoanProductId { get; set; }
       
    }

}
