using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
    public class UpdateCustomerMoralPersonCommand : IRequest<ServiceResponse<UpdateCustomer>>
    {
        public string CustomerId { get; set; }
        public string FirstName { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public bool Active { get; set; }
        public string ActiveStatus { get; set; }
    }

}
