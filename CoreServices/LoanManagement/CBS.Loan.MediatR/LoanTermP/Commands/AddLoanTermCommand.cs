using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanTermP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanTermCommand : IRequest<ServiceResponse<LoanTermDto>>
    {
        public string Name { get; set; }
        public int MinInMonth { get; set; }
        public int MaxInMonth { get; set; }

    }

}
