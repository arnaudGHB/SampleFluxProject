using CBS.UserServiceMGT.Helper;
using MediatR;

namespace CBS.UserServiceMGT.MediatR
{
    public class DeleteUserCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }

        public DeleteUserCommand(string id)
        {
            Id = id;
        }
    }
}
