using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Dto.ChartOfAccountDto;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.ChartOfAccount.MediatR.Queries
{
    public class GetAllAccountAsTreeNodeQuery : IRequest<ServiceResponse<List<TreeNodeDto>>>
    {
        public bool CanPullAllAccount { get; set; } 
    }
}