
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.Customer.Command
{
    public class ActivateDeactivateOnLineMobilePackagesCommand : IRequest<ServiceResponse<UpdateCustomer>>
    {
        public string? CustomerId { get; set; }
        public bool Active { get; set; }
    }
}
