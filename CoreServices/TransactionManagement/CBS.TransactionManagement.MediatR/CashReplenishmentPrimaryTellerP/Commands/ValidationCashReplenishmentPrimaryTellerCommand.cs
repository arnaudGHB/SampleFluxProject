using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Commands
{
    /// <summary>
    /// Represents a command to add a new CashReplenishmentPrimaryTeller.
    /// </summary>
    public class ValidationCashReplenishmentPrimaryTellerCommand : IRequest<ServiceResponse<CashReplenishmentPrimaryTellerDto>>
    {
        public string Id { get; set; }
        public decimal ConfirmedAmount { get; set; }
        public string ApprovedComment { get; set; }
        public string ApprovedStatus { get; set; }
        public CurrencyNotesRequest CurrencyNote { get; set; }
    }

}
