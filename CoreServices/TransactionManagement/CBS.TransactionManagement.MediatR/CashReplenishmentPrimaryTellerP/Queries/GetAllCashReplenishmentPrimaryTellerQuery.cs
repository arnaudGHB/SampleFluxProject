using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Queries
{
    public class GetAllCashReplenishmentPrimaryTellerQuery : IRequest<ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
