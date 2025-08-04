using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.ChartOfAccount.MediatR.Queries
{
    public class GetAllChartOfAccountQuery : IRequest<ServiceResponse<List<ChartOfAccountDto>>>
    {
    }
}