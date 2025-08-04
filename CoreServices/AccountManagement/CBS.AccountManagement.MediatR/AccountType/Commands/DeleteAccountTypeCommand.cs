using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a AddAccountTypeCommand.
    /// </summary>
    public class DeleteAccountRubriqueCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AddAccountTypeCommand to be deleted.
        /// </summary>
       
        public List<string> ListOfOperationEventAttributeId { get; set; }


    }
}