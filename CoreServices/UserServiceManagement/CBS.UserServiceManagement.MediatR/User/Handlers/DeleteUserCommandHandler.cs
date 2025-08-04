using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CBS.UserServiceManagement.MediatR
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ServiceResponse<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork<UserContext> uow,
            ILogger<DeleteUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _uow = uow;
            _logger = logger;
        }

        public async Task<ServiceResponse<bool>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userToDelete = await _userRepository.FindAsync(request.Id);

                if (userToDelete == null)
                {
                    _logger.LogWarning("Attempted to delete a non-existent user with ID {UserId}.", request.Id);
                    return ServiceResponse<bool>.Return404($"User with ID {request.Id} not found.");
                }
                
                _userRepository.Delete(userToDelete.Id);
                
                await _uow.SaveAsync();

                _logger.LogInformation("Successfully soft-deleted user with ID {UserId}.", request.Id);
                
                return ServiceResponse<bool>.ReturnResultWith200(true, "User successfully deleted.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user with ID {UserId}.", request.Id);
                return ServiceResponse<bool>.Return500(ex);
            }
        }
    }
}
