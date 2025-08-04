using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR.Customer.Queries
{
    public class GetAllCustomerAgeCategoriesQuery : IRequest<ServiceResponse<List<CustomerAgeCategoryDto>>>
    {
    }
}
