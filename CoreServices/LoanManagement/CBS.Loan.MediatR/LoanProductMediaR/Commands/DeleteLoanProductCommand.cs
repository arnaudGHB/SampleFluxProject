using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Commands
{

    /// <summary>
    /// Represents a command to delete a customer.
    /// </summary>
    public class DeleteLoanProductCommand : IRequest<ServiceResponse<bool>>
    {
        public string LoanProductId { get; set; }
        public string UserId { get; set; }
    }

}
