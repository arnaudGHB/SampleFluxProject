using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a UpdateOperationEventNameCommand.
    /// </summary>
    public class UpdateOperationEventCommand : IRequest<ServiceResponse<OperationEventDto>>
    {

        public string Id { get; set; }
        public string OperationEventName { get; set; }
        public string Description { get; set; }
        public string EventCode { get; set; }
    }
}