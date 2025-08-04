using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CollateralMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateCollateralCommand : IRequest<ServiceResponse<CollateralDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
