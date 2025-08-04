using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{
    public class EndOfDayPrimaryTellerCommand : IRequest<ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        public CurrencyNotesRequest CurrencyNotes { get; set; }
        public string Comment { get; set; }
        public decimal CashAtHand { get; set; }
        public string ClossedStatus { get; set; }

    }
}
