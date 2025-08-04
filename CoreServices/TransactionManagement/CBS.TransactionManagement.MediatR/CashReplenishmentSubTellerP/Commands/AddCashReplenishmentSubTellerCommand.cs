using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>
    public class AddCashReplenishmentSubTellerCommand : IRequest<ServiceResponse<SubTellerCashReplenishmentDto>>
    {
        public decimal RequestedAmount { get; set; }
        public string Requetcomment { get; set; }
    }

}
