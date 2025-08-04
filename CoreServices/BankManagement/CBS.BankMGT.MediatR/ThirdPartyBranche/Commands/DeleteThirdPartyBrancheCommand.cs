using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a ThirdPartyBranche.
    /// </summary>
    public class DeleteThirdPartyBrancheCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ThirdPartyBranche to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
