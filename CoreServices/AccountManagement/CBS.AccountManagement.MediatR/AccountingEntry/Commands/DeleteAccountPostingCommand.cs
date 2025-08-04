

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a customer.
    /// </summary>
    public class DeleteAccountPostingCommand : IRequest<ServiceResponse<List<AccountingEntryDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customer to be deleted.
        /// </summary>
        public string referenceId { get; set; }
    }
}