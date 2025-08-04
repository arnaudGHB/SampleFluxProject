using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class GetTransactionByTransactionReferenceQuery : IRequest<ServiceResponse<List<TransactionDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Transaction to be retrieved.
        /// </summary>
        public string TransactionReference { get; set; }
    }
}
