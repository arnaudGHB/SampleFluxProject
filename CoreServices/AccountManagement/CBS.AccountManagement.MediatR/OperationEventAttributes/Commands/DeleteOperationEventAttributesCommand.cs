using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a OperationEventNameAttributes.
    /// </summary>
    public class DeleteOperationEventAttributesCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the OperationEventNameAttributes to be deleted.
        /// </summary>
        public string Id { get; set; }
    }
}