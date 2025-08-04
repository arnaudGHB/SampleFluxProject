using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to delete a LinkAccountTypeAccountFeature.
    /// </summary>
    public class DeleteLinkAccountTypeAccountFeatureCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LinkAccountTypeAccountFeature to be deleted.
        /// </summary>
        public string Id { get; set; }
    }
}