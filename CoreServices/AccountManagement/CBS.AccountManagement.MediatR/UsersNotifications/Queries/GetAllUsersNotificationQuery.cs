using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class GetAllUsersNotificationQuery : IRequest<ServiceResponse<List<UsersNotificationDto>>>
    {

    }
    public class GetAllUsersNotificationByBranchQuery : IRequest<ServiceResponse<List<UsersNotificationDto>>>
    {

    }
}