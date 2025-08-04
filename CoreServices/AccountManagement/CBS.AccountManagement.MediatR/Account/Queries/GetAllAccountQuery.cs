using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllAccountQuery : IRequest<ServiceResponse<List<AccountDto>>>
    {
        
    }
    public class GetAllLiasonAccountByBranchIdQuery : IRequest<ServiceResponse<List<AccountDto>>>
    {
      
    }
}