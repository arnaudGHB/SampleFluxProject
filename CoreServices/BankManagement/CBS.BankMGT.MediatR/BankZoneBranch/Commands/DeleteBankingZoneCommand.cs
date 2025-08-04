
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a BankZoneBranch.
    /// </summary>
    public class DeleteBankZoneBranchCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the BankZoneBranch to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
