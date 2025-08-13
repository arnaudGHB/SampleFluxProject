using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.Commands.ReversalRequestP
{

    /// <summary>
    /// Represents a command to delete a CashReplenishment.
    /// </summary>
    public class DeleteReversalRequestCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the CashReplenishment to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
