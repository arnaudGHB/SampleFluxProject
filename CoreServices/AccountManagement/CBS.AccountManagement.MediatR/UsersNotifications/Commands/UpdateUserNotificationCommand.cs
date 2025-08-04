using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;


namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateOperationEventNameCommand.
    /// </summary>
    public class UpdateUserNotificationCommand : IRequest<ServiceResponse<UsersNotificationDto>>
    {

        public string Id { get; set; }
        public string Action { get; set; }
        public string ActionId { get; set; }
        public DateTime CreateAt { get; set; }
        public string ActionUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSeen { get; set; } = false;
    }
}