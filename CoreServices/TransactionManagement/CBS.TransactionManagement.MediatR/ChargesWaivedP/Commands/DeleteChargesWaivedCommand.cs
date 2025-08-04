using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands.ChargesWaivedP
{

    /// <summary>
    /// Represents a command to delete a WithdrawalLimits.
    /// </summary>
    public class DeleteChargesWaivedCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the WithdrawalLimits to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
