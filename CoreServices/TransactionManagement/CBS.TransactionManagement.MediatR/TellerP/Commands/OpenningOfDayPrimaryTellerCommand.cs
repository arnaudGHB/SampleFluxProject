using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{
    /// <summary>
    /// Represents a command to update a Teller.
    /// </summary>
    public class ReOpenningOfDayPrimaryTellerCommand : IRequest<ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        public decimal InitialAmount { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }
    }

}
