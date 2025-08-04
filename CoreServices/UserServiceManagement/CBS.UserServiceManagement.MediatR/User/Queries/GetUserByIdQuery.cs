using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class GetUserByIdQuery : IRequest<ServiceResponse<UserDto>>
    {
        public string Id { get; set; }
    }
}