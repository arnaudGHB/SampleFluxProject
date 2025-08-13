using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands
{
    /// <summary>
    /// Represents a command to update a CashReplenishment.
    /// </summary>
    public class ValidateCashReplenishmentSubTellerCommand : IRequest<ServiceResponse<SubTellerCashReplenishmentDto>>
    {
        public string Id { get; set; }
        public decimal ConfirmedAmount { get; set; }
        public string ApprovedComment { get; set; }
        public string ApprovedStatus { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }


    }

}
