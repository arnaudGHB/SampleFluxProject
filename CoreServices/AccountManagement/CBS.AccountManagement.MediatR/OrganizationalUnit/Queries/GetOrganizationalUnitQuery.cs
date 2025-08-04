using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific GetOrganizationalUnit by its unique identifier.
    /// </summary>
    public class GetOrganizationalUnitQuery : IRequest<ServiceResponse<OrganizationalUnitDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the AccountCategory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}