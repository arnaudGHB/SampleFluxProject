using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanApplicationFeeCommand : IRequest<ServiceResponse<LoanApplicationFeeDto>>
    {
        public string Id { get; set; }
        public string FeeId { get; set; }
        public string FeeRangeId { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        public string LoanApplicationId { get; set; }

    }

}
