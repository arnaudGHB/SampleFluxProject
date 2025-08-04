using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllAccountByBranchIDQuery : IRequest<ServiceResponse<List<AccountDto>>>
    {
        public string BranchId { get; set; }
    }
}