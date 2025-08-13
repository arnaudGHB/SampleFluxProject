using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{

    /// <summary>
    /// Represents a command to delete a Transaction.
    /// </summary>
    public class DeleteTransactionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Transaction to be deleted.
        /// </summary>
        public string Id { get; set; }
        public string UserId { get; set; }
    }

}
