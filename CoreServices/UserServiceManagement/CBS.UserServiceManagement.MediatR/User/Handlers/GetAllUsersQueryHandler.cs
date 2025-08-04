// --- USINGS NÉCESSAIRES ---
using AutoMapper;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.MediatR; // Namespace correct pour la Query
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore; // Nécessaire pour ToListAsync
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// --- NAMESPACE CONFORME À LA STRUCTURE ---
namespace CBS.UserServiceManagement.MediatR
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ServiceResponse<List<UserDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllUsersQueryHandler> _logger;

        public GetAllUsersQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            ILogger<GetAllUsersQueryHandler> logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Appeler le repository pour récupérer tous les utilisateurs
                var users = await _userRepository.All.ToListAsync(cancellationToken);

                // 2. Mapper la liste d'entités User vers une liste de UserDto
                var userDtos = _mapper.Map<List<UserDto>>(users);

                _logger.LogInformation("Successfully retrieved {UserCount} users.", users.Count);

                // 3. Retourner la réponse de succès avec la liste
                return ServiceResponse<List<UserDto>>.ReturnResultWith200(userDtos);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                // Utiliser la méthode factory standard pour une erreur 500
                return ServiceResponse<List<UserDto>>.Return500(ex);
            }
        }
    }
}