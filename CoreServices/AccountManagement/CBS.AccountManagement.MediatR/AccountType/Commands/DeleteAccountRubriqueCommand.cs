using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a AddAccountTypeCommand.
    /// </summary>
    public class DeleteAccountTypeCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AddAccountTypeCommand to be deleted.
        /// </summary>
        public string Id { get; set; }

        public string IdType { get; set; } = "ACCOUNTTYPE";
    }
}