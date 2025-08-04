using CBS.CUSTOMER.DATA.Dto;

using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer secret.
    /// </summary>
        public class UpdateCustomerSecretCommand : IRequest<ServiceResponse<UpdateCustomer>>
        {

        public string? SecretQuestion { get; set; }
        public string? SecretAnswer { get; set; }
        public string? CustomerId { get; set; }
    
        }

}
