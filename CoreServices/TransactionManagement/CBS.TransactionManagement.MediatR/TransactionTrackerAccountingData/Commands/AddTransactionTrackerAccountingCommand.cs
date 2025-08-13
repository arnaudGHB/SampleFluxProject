
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands
{
    /// <summary>
    /// Represents a command to add a new AuditTrail.
    /// </summary>
    public class AddTransactionTrackerAccountingCommand : IRequest<ServiceResponse<TransactionTrackerAccountingDto>>
    {
        public CommandDataType CommandDataType { get; set; }
        public string CommandJsonObject { get; set; }
        public string TransactionReferenceId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? DestinationUrl { get; set; }
        public string? SourceUrl { get; set; }
        public UserInfoToken UserInfoToken { get; set; }
        public bool IsBG { get; set; }
        public AddTransactionTrackerAccountingCommand()
        {
            SourceUrl="N/A";
        }

    }

}
