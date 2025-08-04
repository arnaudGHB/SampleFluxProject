using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a customer document.
    /// </summary>
    public class DeleteCustomerDocumentCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the customer document to be deleted.
        /// </summary>
        public  string Id { get; set; }
    }

}
