using CBS.UserServiceMGT.Data;
using CBS.UserServiceMGT.Helper;
using MediatR;

namespace CBS.UserServiceMGT.MediatR
{
    public class GetUserByIdQuery : IRequest<ServiceResponse<UserDto>>
    {
        public string Id { get; set; }
    }
}