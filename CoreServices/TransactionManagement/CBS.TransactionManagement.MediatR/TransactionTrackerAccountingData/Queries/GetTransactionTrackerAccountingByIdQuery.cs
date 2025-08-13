using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries
{
    /// <summary>
    /// Query for retrieving a TransactionTrackerAccounting record by ID.
    /// </summary>
    public class GetTransactionTrackerAccountingByIdQuery : IRequest<ServiceResponse<TransactionTrackerAccountingDto>>
    {
        /// <summary>
        /// The ID of the record to retrieve.
        /// </summary>
        public string Id { get; set; }
    }
}
