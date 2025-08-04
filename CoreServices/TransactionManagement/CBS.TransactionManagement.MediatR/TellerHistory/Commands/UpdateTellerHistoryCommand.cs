using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new TellerHistory.
    /// </summary>
    public class UpdateTransferCommand : IRequest<ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        public string Id { get; set; }
        public decimal EndOfDayAmount { get; set; }
        public decimal BalanceAtHand { get; set; }
        public string CurrencyNotesId { get; set; }
    }

}
