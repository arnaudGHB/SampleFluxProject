using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries
{
    public class GetAllCashReplenishmentSubTellerPendingQuery : IRequest<ServiceResponse<List<SubTellerCashReplenishmentDto>>>
    {
       
    }
}
