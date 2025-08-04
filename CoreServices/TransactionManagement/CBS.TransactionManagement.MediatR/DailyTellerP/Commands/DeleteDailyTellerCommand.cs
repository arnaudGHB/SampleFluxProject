using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyTellerP.Commands
{

    /// <summary>
    /// Represents a command to delete a DepositLimit.
    /// </summary>
    public class DeleteDailyTellerCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the DepositLimit to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
