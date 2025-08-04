using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR
{

    /// <summary>
    /// Represents a command to delete a group customer.
    /// </summary>
    public class DeleteGroupCustomerCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the group customer to be deleted.
        /// </summary>
        public  string Id { get; set; }
    }

}
