using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries
{
    public class GetAllCashReplenishmentQuery : IRequest<ServiceResponse<List<SubTellerCashReplenishmentDto>>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }
}
