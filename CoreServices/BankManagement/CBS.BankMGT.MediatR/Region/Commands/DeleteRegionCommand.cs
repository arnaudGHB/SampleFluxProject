using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Region.
    /// </summary>
    public class DeleteRegionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Region to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
