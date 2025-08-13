using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries
{
    public class GetAllCashReplenishmentPrimaryTellerPendingQuery : IRequest<ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>>
    {
       
    }
}
