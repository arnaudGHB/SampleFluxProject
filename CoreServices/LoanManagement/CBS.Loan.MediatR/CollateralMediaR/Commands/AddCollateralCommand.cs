using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CollateralMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddCollateralCommand : IRequest<ServiceResponse<CollateralDto>>
    {
        public string Name { get; set; }
    }

}
