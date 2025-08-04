using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.OldLoanConfiguration.Commands
{

    /// <summary>
    /// Represents a command to delete a CloseFeeParameter.
    /// </summary>
    public class DeleteOldLoanAccountingMapingCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the CloseFeeParameter to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
