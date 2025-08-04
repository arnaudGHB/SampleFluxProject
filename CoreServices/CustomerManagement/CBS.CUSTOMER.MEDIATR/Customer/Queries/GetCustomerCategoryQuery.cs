using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Customer by its unique identifier.
    /// </summary>
    public class GetCustomerCategoryQuery : IRequest<ServiceResponse<CreateCustomerCategory>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Customer to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
