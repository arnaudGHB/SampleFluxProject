using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class GetCustomerByAccountNumber : IRequest<ServiceResponse<CustomerKYCDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Transaction to be retrieved.
        /// </summary>
        public string AccountNumber { get; set; }
    }
}
