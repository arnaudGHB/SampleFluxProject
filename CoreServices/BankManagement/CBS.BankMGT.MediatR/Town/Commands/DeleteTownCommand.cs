using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Town.
    /// </summary>
    public class DeleteTownCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Town to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
