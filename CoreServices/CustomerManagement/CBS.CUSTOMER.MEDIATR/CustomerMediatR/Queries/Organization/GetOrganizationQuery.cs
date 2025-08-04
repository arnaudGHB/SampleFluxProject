using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Organization by its unique identifier.
    /// </summary>
    public class GetOrganizationQuery : IRequest<ServiceResponse<DATA.Entity.Organization>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Organization to be retrieved.
        /// </summary>
        public string OrganizationId { get; set; }
    }
}
