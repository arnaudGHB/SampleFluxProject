using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Commands
{

    /// <summary>
    /// Represents a command to delete a TransferLimits.
    /// </summary>
    public class DeleteMemberAccountActivationPolicyCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the TransferLimits to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
