using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Commands
{
    public class EndOfDaySubTellerCommand : IRequest<ServiceResponse<SubTellerProvioningHistoryDto>>
    {
        public CurrencyNotesRequest CurrencyNotes { get; set; }
        public string Comment { get; set; }
        public decimal CashAtHand { get; set; }

    }


}
