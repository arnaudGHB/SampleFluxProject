using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a EconomicActivity.
    /// </summary>
    public class DeleteEconomicActivityCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the EconomicActivity to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
