using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries
{
    /// <summary>
    /// Query for retrieving the last X transactions.
    /// </summary>
    public class GetLastXTransactionsQuery : IRequest<ServiceResponse<List<TransactionTrackerAccountingDto>>>
    {
        /// <summary>
        /// Number of transactions to retrieve.
        /// </summary>
        public int TransactionCount { get; set; }
    }
}
