using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllTrailBalanceUploudQuery : IRequest<ServiceResponse<List<TrailBalanceUploudDto>>>
    {

    }
}