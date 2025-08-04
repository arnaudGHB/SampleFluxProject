using CBS.SystemConfiguration.Helper;

using MediatR;

namespace CBS.SystemConfiguration.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a DeleteOperationEventNameCommand.
    /// </summary>
    public class DeleteDivisionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the OperationEventName to be deleted.
        /// </summary>
        public string Id { get; set; }
    }
}