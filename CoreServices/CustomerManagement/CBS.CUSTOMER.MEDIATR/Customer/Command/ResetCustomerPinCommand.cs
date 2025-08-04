
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Entity;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new customer.
    /// </summary>
    public class ResetCustomerPinCommand : IRequest<ServiceResponse<CreateCustomer>>
    {

        public string? Phone { get; set; }

    }

}
