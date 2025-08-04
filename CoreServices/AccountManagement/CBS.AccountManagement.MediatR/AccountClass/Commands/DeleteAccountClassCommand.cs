using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a SubAccountCategory.
    /// </summary>
    public class DeleteAccountClassCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the SubAccountCategory to be deleted.
        /// </summary>
        public string Id { get; set; }
    }
}