using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.PeriodMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddPeriodCommand : IRequest<ServiceResponse<PeriodDto>>
    {
        //public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
