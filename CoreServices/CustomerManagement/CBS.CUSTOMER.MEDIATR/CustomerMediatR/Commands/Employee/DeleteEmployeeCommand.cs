using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Employee.
    /// </summary>
    public class DeleteEmployeeCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Employee to be deleted.
        /// </summary>
        public string Id { get; set; }
        public string UserId { get; set; }
      
    }

}
