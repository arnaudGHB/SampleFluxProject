
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands
{
    /// <summary>
    /// Represents a command to add a new AuditTrail.
    /// </summary>
    public class UpdateTransactionTrackerAccountingCommand : IRequest<ServiceResponse<TransactionTrackerAccountingDto>>
    {
        public string Id { get; set; }
        public bool HasPassed { get; set; }
        public int NumberOfRetry { get; set; }
        public DateTime DatePassed { get; set; }

    }

}
