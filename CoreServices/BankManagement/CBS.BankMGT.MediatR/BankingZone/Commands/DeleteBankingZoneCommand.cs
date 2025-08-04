
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a BankingZone.
    /// </summary>
    public class DeleteBankingZoneCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the BankingZone to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
