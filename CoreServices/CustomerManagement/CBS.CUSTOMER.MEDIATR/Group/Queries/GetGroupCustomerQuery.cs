using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific GroupCustomer by its unique identifier.
    /// </summary>
    public class GetGroupCustomerQuery : IRequest<ServiceResponse<DATA.Entity.GroupCustomer>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the GroupCustomer to be retrieved.
        /// </summary>
        public string GroupCustomerId { get; set; }
    }
}
