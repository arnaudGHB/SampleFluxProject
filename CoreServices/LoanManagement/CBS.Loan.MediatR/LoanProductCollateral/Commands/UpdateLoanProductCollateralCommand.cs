using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductCollateralMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanProductCollateralCommand : IRequest<ServiceResponse<LoanProductCollateralDto>>
    {
        public string Id { get; set; }
        public string CollateralId { get; set; }
        public string LoanProductId { get; set; }
        public string LoanProductCollateralTag { get; set; }
        public decimal MinimumValueRate { get; set; }
        public decimal MaximumValueRate { get; set; }
    }

}
