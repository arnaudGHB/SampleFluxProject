using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a AccountFeature.
    /// </summary>
    public class DeleteEntryTempDataCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountFeature to be deleted.
        /// </summary>
        public string Id { get; set; }
    }
}