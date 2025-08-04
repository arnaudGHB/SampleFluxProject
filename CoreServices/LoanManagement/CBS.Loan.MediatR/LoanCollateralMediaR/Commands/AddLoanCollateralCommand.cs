using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanCollateralMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanCollateralCommand : IRequest<ServiceResponse<LoanApplicationCollateralDto>>
    {
        public string LoanApplicationId { get; set; }
        public string LoanProductCollateralId { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string? Location { get; set; }
        public string? Address { get; set; }
        public string? Reference { get; set; }
    }

}
