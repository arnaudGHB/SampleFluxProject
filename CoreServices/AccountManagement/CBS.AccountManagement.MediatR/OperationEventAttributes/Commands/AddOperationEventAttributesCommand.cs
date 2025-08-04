using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new OperationEventNameAttributes.
    /// </summary>
    public class AddOperationEventAttributesCommand : IRequest<ServiceResponse<OperationEventAttributesDto>>
    {
        public string? OperationEventAttributeCode { get; set; }  
        public string Name { get; set; }
        public string Description { get; set; }
        public string? OperationEventId { get; set; }
    }
}