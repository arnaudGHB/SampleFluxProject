using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a customer.
    /// </summary>
    public class DeleteCustomerCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customer to be deleted.
        /// </summary>
        public  string? Id { get; set; }
        public  string? UserId { get; set; }
    }

}
