using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific SavingProduct by its unique identifier.
    /// </summary>
    public class GetSavingProductQuery : IRequest<ServiceResponse<SavingProductDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the SavingProduct to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
