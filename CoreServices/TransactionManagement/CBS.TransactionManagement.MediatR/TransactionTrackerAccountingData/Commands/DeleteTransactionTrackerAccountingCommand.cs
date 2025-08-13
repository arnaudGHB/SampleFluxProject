using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands
{

    /// <summary>
    /// Represents a command to delete a AuditTrail.
    /// </summary>
    public class DeleteTransactionTrackerAccountingCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AuditTrail to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
