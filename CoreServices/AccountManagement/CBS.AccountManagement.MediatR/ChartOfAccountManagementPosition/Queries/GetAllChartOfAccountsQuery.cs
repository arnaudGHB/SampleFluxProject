using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllChartOfAccountMPsQuery : IRequest<ServiceResponse<List<ChartOfAccountStateDto>>>
    {
    }

    public class GetAllChartOfAccountMPUsedByBranchQuery : IRequest<ServiceResponse<List<ChartOfAccountStateDto>>>
    {
        public string BranchId { get; set; }
    }
}