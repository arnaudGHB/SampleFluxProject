using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{
    /// <summary>
    /// Represents a command to update a Teller.
    /// </summary>
    public class ProvisionPrimaryTellerCommand : IRequest<ServiceResponse<TellerDto>>
    {
        public string ReplenishmentId { get; set; }
        public string Note { get; set; } = "Primary Teller provisioning to start the day";
        public decimal Amount { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }
    }

}
