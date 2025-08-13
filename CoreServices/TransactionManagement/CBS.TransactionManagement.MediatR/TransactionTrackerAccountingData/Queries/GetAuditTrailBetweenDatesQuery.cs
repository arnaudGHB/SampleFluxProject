using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries
{
    /// <summary>
    /// Query for retrieving TransactionTrackerAccounting records between specified dates.
    /// </summary>
    public class GetTransactionTrackerAccountingByDateRangeQuery : IRequest<ServiceResponse<List<TransactionTrackerAccountingDto>>>
    {
        /// <summary>
        /// The start date for the range.
        /// </summary>
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
