using CBS.CUSTOMER.DATA.Entity.Customers;
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto.Customers;

namespace CBS.CUSTOMER.MEDIATR.Customer.Command
{
    public class AddCustomerAgeCategoryCommand : IRequest<ServiceResponse<CustomerAgeCategoryDto>>
    {
        public string Name { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}
