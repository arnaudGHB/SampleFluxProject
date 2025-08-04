using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
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
