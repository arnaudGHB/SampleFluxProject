using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific OrganizationCustomer by its unique identifier.
    /// </summary>
    public class GetOrganizationCustomerQuery : IRequest<ServiceResponse<DATA.Entity.OrganizationCustomer>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the OrganizationCustomer to be retrieved.
        /// </summary>
        public string? OrganizationCustomerId { get; set; }
    }
}
