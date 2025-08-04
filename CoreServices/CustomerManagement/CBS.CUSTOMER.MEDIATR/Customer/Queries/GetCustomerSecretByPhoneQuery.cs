using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Customer by its phone.
    /// </summary>
    public class GetCustomerSecretByPhoneQuery : IRequest<ServiceResponse<GetCustomerSecret>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Customer to be retrieved.
        /// </summary>
        public string? CustomerId { get; set; }
    }
}
