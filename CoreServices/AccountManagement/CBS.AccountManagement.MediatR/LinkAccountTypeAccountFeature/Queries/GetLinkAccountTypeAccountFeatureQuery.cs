using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific LinkAccountTypeAccountFeature by its unique identifier.
    /// </summary>
    public class GetLinkAccountTypeAccountFeatureQuery : IRequest<ServiceResponse<LinkAccountTypeAccountFeatureDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the LinkAccountTypeAccountFeature to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}