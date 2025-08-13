using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{

    /// <summary>
    /// Represents a command to delete a Teller.
    /// </summary>
    public class DeleteTellerCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Teller to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
