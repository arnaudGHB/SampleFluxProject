using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllAccountTypeQuery : IRequest<ServiceResponse<List<AccountTypeDto>>>
    {
    }
}