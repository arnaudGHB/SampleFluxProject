using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllCashMovementTrackingConfigurationQuery : IRequest<ServiceResponse<List<CashMovementTrackingConfigurationDto>>>
    {

    }
}