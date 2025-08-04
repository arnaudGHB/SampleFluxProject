using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{

    /// <summary>
    /// Represents a command to dis activate a customer.
    /// </summary>
    public class DisActivateCustomerCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customer to be deleted.
        /// </summary>
        public required string CustomerId { get; set; }
        public required bool activate { get; set; }
   
    }

}
