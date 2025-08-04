using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Division.
    /// </summary>
    public class DeleteDivisionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Division to be deleted.
        /// </summary>
        public string Id { get; set; }
    }

}
