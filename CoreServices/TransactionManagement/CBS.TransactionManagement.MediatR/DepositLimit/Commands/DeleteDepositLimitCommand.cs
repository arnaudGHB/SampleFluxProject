using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{

    /// <summary>
    /// Represents a command to delete a DepositLimit.
    /// </summary>
    public class DeleteDepositLimitCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the DepositLimit to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
