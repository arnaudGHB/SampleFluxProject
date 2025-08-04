using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Customer by its unique identifier.
    /// </summary>
    public class GetCustomerByMsisdnQuery : IRequest<ServiceResponse<CustomerDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Customer to be retrieved.
        /// </summary>
        public string TelephoneNumber { get; set; }
    }
}
