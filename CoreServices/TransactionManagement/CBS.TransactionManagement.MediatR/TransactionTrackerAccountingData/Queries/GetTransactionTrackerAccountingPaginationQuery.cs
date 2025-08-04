using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Helper.Pagging;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries
{
    /// <summary>
    /// Query to retrieve paginated TransactionTrackerAccounting data with optional filters.
    /// </summary>
    public class GetTransactionTrackerAccountingPaginationQuery : IRequest<ServiceResponse<PaginatedList<TransactionTrackerAccountingDto>>>
    {
        /// <summary>
        /// The page number to retrieve.
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Optional filters as key-value pairs.
        /// </summary>
        public Dictionary<string, object>? Filters { get; set; }
    }

}
