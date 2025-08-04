using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Transaction by its unique identifier.
    /// </summary>
    public class GetTellerAccountBalanceQuery : IRequest<ServiceResponse<TillOpenAndCloseOfDayDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Transaction to be retrieved.
        /// </summary>
        public string TellerId { get; set; }
        public bool IsPrimary { get; set; }
        public bool HasValue { get; set; }
        public bool IsCloseOfDayPrimaryTeller { get; set; }

    }
}
