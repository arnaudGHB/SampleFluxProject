using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{

    /// <summary>
    /// Represents a command to delete a EntryFeeParameter.
    /// </summary>
    public class DeleteEntryFeeParameterCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the EntryFeeParameter to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
