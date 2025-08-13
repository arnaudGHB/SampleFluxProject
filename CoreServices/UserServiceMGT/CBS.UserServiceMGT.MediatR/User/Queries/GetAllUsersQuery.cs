using CBS.UserServiceMGT.Data;
using CBS.UserServiceMGT.Helper;
using MediatR;
using System.Collections.Generic;

namespace CBS.UserServiceMGT.MediatR
{
    public class GetAllUsersQuery : IRequest<ServiceResponse<List<UserDto>>>
    {
    }
}