using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CycleNameMediaR.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateLoanProductCategoryCommand : IRequest<ServiceResponse<LoanProductCategoryDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

    }

}
