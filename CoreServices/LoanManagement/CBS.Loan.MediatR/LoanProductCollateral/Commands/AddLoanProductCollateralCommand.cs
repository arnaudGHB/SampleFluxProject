using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductCollateralMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanProductCollateralCommand : IRequest<ServiceResponse<LoanProductCollateralDto>>
    {
        public string CollateralId { get; set; }
        public string LoanProductId { get; set; }
        public string LoanProductCollateralTag { get; set; }
        public decimal MinimumValueRate { get; set; }
        public decimal MaximumValueRate { get; set; }

    }

}
