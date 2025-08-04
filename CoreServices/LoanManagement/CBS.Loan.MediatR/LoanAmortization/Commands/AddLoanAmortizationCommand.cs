using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanAmortizationCommand : IRequest<ServiceResponse<List<LoanAmortizationDto>>>
    {
        public string? LoanId { get; set; }
        public string? LoanApplicationId { get; set; }
        public LoanApplication? LoanApplication { get; set; }
    }

}
