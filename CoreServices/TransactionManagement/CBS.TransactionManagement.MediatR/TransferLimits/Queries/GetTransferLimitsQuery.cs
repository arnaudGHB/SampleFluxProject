using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific TransferLimits by its unique identifier.
    /// </summary>
    public class GetTransferLimitsQuery : IRequest<ServiceResponse<TransferParameterDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the TransferLimits to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
