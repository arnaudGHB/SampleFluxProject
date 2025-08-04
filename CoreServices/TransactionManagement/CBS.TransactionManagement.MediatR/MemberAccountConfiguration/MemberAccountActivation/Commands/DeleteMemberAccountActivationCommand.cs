using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MemberAccountConfiguration.Commands
{


    /// <summary>
    /// Represents a command to delete a TransferLimits.
    /// </summary>
    public class DeleteMemberAccountActivationCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the TransferLimits to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
