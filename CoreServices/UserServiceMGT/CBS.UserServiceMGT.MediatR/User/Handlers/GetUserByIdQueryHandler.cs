// --- USINGS N�CESSAIRES ---
using AutoMapper;
using CBS.UserServiceMGT.Data;
using CBS.UserServiceMGT.Helper;
using CBS.UserServiceMGT.MediatR; // Namespace correct pour la Query
using CBS.UserServiceMGT.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

// --- NAMESPACE CONFORME � LA STRUCTURE ---
namespace CBS.UserServiceMGT.MediatR
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

                // 2. G�rer le cas o� l'utilisateur n'est pas trouv�
                if (user == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found.", request.Id);
                    // Utiliser la m�thode factory standard pour une erreur 404
                    return ServiceResponse<UserDto>.Return404($"User with ID {request.Id} not found.");
                }

                // 3. Mapper l'entit� User vers un UserDto pour la r�ponse
                var userDto = _mapper.Map<UserDto>(user);

                _logger.LogInformation("Successfully retrieved user with ID {UserId}.", request.Id);

                // 4. Retourner une r�ponse de succ�s standard
                return ServiceResponse<UserDto>.ReturnResultWith200(userDto);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving user with ID {UserId}.", request.Id);
                // Utiliser la m�thode factory standard pour une erreur 500
                return ServiceResponse<UserDto>.Return500(ex);
            }
        }
    }
}