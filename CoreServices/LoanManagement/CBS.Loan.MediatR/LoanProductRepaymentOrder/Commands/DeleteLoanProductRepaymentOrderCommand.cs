using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Commands
{

    /// <summary>
    /// Represents a command to delete a customer.
    /// </summary>
    public class DeleteLoanProductRepaymentOrderCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
    }

}
