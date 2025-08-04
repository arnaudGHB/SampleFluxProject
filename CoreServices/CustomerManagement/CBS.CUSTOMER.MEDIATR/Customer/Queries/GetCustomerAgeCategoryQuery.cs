using CBS.CUSTOMER.DATA.Entity.Customers;
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR.Customer.Queries
{
    // Get Query
    public class GetCustomerAgeCategoryQuery : IRequest<ServiceResponse<CustomerAgeCategoryDto>>
    {
        public string Id { get; set; }
    }

}
