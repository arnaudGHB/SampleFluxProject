using CBS.CUSTOMER.DATA.Dto.Customers;
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.MEDIATR.Customer.Queries
{

    /// <summary>
    /// Represents a query to retrieve a specific Customer Date Of Birth by its unique identifier.
    /// </summary>
    public class GetCustomerDateOfBirthQuery : IRequest<ServiceResponse<GetCustomerDateOfBirthDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Customer  to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
   
}
