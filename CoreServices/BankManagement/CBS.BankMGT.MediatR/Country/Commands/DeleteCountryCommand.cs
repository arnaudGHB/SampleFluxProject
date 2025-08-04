using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Country.
    /// </summary>
    public class DeleteCountryCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Country to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
