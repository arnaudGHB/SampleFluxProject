using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Commands
{

    /// <summary>
    /// Represents a command to delete a CashReplenishment.
    /// </summary>
    public class DeleteRemittanceCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the CashReplenishment to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
