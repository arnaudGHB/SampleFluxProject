using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AddOperationEventNameCommand.
    /// </summary>
    public class AddOperationEventCommand : IRequest<ServiceResponse<OperationEventDto>>
    {

        public string? OperationEventName { get; set; }
        public string? Description { get; set; }
        public string EventCode { get; set; }
        public bool HasMultipleEntries { get; set; }

    }
}