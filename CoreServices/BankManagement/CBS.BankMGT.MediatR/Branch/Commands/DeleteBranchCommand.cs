using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Branch.
    /// </summary>
    public class DeleteBranchCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Branch to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
