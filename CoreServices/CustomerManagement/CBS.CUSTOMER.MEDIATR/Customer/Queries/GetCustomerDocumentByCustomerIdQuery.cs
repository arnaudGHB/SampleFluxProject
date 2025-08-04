using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific CustomerDocument by its unique identifier.
    /// </summary>
    public class GetCustomerDocumentByCustomerIdQuery : IRequest<ServiceResponse<List<GetCustomerDocument>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the CustomerDocument to be retrieved by CustomerId.
        /// </summary>
        public string CustomerId { get; set; }
    }
}
