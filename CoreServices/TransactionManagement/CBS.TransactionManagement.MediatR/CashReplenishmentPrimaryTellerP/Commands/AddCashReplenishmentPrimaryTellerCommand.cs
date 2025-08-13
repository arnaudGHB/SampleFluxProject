using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Commands
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>
    public class AddCashReplenishmentPrimaryTellerCommand : IRequest<ServiceResponse<CashReplenishmentPrimaryTellerDto>>
    {
        public decimal RequestedAmount { get; set; }
        public string Requetcomment { get; set; }
    }

}
