using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllAccountClassesQuery : IRequest<ServiceResponse<List<AccountClassDto>>>
    {
    }
}