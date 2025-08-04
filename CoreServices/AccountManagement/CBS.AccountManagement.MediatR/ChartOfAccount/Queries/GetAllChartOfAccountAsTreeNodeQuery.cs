using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.ChartOfAccount.MediatR.Queries
{
    public class GetAllChartOfAccountAsTreeNodeQuery : IRequest<ServiceResponse<List<TreeNodeDto>>>
    {
    }
}