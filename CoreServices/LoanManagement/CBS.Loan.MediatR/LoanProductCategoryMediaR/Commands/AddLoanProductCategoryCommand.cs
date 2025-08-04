using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CycleNameMediaR.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddLoanProductCategoryCommand : IRequest<ServiceResponse<LoanProductCategoryDto>>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }

}
