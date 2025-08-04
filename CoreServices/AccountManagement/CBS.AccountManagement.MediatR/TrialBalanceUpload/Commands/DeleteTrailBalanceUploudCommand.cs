using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a DeleteOperationEventNameCommand.
    /// </summary>
    public class DeleteTrailBalanceUploudCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the OperationEventName to be deleted.
        /// </summary>
        public string Id { get; set; }
    }
}