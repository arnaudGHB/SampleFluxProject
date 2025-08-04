using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Queries
{
    /// <summary>
    /// Query for retrieving a transaction by its reference.
    /// </summary>
    public class GetTransactionByReferenceQuery : IRequest<ServiceResponse<TransactionTrackerAccountingDto>>
    {
        /// <summary>
        /// The transaction reference to retrieve.
        /// </summary>
        public string TransactionReference { get; set; }
    }
}
