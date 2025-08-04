using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.RescheduleLoanMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddRescheduleLoanCommand : IRequest<ServiceResponse<RescheduleLoanDto>>
    {
        //public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
