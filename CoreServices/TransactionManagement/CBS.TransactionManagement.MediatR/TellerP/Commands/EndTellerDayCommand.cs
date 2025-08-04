using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{
    /// <summary>
    /// Represents a command to update a Teller.
    /// </summary>
    public class EndTellerDayCommand : IRequest<ServiceResponse<TellerDto>>
    {
        public string Id { get; set; }
        public decimal EndOfDayAmount { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }
        public string Comment { get; set; }
        public decimal CashAtHand { get; set; }
        public decimal InitialAmount { get; set; }

    }

}
