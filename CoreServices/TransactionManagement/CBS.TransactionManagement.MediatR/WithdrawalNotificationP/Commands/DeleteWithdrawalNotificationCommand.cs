using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands.WithdrawalNotificationP
{

    /// <summary>
    /// Represents a command to delete a WithdrawalLimits.
    /// </summary>
    public class DeleteWithdrawalNotificationCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the WithdrawalLimits to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
