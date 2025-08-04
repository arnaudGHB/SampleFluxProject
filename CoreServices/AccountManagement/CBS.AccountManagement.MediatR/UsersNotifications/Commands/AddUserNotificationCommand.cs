using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;


namespace CBS.AccountingManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddUserNotificationCommand : IRequest<ServiceResponse<UsersNotificationDto>>
    {

        public string Action { get; set; }
        public string ActionId { get; set; }
        public string ActionUrl { get; set; }

    }
}