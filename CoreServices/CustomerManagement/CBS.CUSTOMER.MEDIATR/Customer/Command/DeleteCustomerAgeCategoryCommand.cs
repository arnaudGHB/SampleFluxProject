using MediatR;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.CUSTOMER.MEDIATR.Customer.Command
{
    // Delete Command
    public class DeleteCustomerAgeCategoryCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
    }

}
