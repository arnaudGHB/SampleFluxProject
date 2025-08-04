using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a ThirdPartyInstitution.
    /// </summary>
    public class DeleteThirdPartyInstitutionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the ThirdPartyInstitution to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
