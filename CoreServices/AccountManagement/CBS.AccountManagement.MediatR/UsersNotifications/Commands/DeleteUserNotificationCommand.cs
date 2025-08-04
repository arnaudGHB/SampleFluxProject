using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;


namespace CBS.AccountingManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a DeleteOperationEventNameCommand.
    /// </summary>
    public class DeleteUserNotificationCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the OperationEventName to be deleted.
        /// </summary>
        public string Id { get; set; }
    }
}