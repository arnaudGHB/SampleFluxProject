using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{

    /// <summary>
    /// Represents a command to delete a ReopenFeeParameter.
    /// </summary>
    public class DeleteReopenFeeParameterCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ReopenFeeParameter to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
