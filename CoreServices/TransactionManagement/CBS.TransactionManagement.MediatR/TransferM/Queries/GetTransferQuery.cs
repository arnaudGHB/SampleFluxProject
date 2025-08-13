using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.TransferM.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Teller by its unique identifier.
    /// </summary>
    public class GetTransferQuery : IRequest<ServiceResponse<TransferDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Teller to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
