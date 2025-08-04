using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeP.Commands
{

    /// <summary>
    /// Represents a command to delete a customer.
    /// </summary>
    public class DeleteFeeCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customer to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
