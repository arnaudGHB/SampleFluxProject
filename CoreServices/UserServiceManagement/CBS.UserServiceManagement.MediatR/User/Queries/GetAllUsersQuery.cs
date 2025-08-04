using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using MediatR;
using System.Collections.Generic;

namespace CBS.UserServiceManagement.MediatR
{
    public class GetAllUsersQuery : IRequest<ServiceResponse<List<UserDto>>>
    {
    }
}