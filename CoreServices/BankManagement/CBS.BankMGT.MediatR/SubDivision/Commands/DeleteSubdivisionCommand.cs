using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Subdivision.
    /// </summary>
    public class DeleteSubdivisionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Subdivision to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
