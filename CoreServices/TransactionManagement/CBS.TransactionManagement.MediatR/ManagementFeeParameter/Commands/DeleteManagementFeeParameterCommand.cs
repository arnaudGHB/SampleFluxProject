using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{

    /// <summary>
    /// Represents a command to delete a ManagementFeeParameter.
    /// </summary>
    public class DeleteManagementFeeParameterCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ManagementFeeParameter to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
