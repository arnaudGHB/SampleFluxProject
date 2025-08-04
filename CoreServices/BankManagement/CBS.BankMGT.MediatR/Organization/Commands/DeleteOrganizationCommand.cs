using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Organization.
    /// </summary>
    public class DeleteOrganizationCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Organization to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
