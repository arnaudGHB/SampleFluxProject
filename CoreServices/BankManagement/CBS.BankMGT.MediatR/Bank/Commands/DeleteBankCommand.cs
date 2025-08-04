using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Bank.
    /// </summary>
    public class DeleteBankCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Bank to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
