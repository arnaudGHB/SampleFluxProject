// --- USINGS NÉCESSAIRES ---
using AutoMapper;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR; // Namespace correct pour la Query
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

// --- NAMESPACE CONFORME À LA STRUCTURE ---
namespace CBS.UserServiceManagement.MediatR
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ServiceResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUserByIdQueryHandler> _logger;

        public GetUserByIdQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<GetUserByIdQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Appeler le repository pour trouver l'utilisateur par son ID
                var user = await _userRepository.FindAsync(request.Id);

                // 2. Gérer le cas où l'utilisateur n'est pas trouvé
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", request.Id);
                    // Utiliser la méthode factory standard pour une erreur 404
                    return ServiceResponse<UserDto>.Return404($"User with ID {request.Id} not found.");
                }

                // 3. Mapper l'entité User vers un UserDto pour la réponse
                var userDto = _mapper.Map<UserDto>(user);

                _logger.LogInformation("Successfully retrieved user with ID {UserId}.", request.Id);

                // 4. Retourner une réponse de succès standard
                return ServiceResponse<UserDto>.ReturnResultWith200(userDto);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID {UserId}.", request.Id);
                // Utiliser la méthode factory standard pour une erreur 500
                return ServiceResponse<UserDto>.Return500(ex);
            }
        }
    }
}