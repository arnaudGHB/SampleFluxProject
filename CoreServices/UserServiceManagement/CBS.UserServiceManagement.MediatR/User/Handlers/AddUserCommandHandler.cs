using AutoMapper;
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

// --- CORRECTION : Le namespace est conservé tel quel, conformément au modèle ---
using AutoMapper;
using CBS.UserServiceManagement.Common;
using CBS.UserServiceManagement.Data;
using CBS.UserServiceManagement.Domain;
using CBS.UserServiceManagement.Helper;
using CBS.UserServiceManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

// --- CORRECTION : Le namespace est conservé tel quel, conformément au modèle ---
namespace CBS.UserServiceManagement.MediatR
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, ServiceResponse<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork<UserContext> _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<AddUserCommandHandler> _logger;

        public AddUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork<UserContext> uow,
            IMapper mapper,
            ILogger<AddUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<UserDto>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var emailExists = await _userRepository.FindBy(u => u.Email.ToLower() == request.Email.ToLower()).AnyAsync(cancellationToken);
                if (emailExists)
                {
                    var errorMessage = $"A user with the email '{request.Email}' already exists.";
                    
                    // --- CORRECTION : Utilisation du template de message pour le logging ---
                    _logger.LogWarning("A user with the email {Email} already exists.", request.Email);
                    
                    return ServiceResponse<UserDto>.Return409(errorMessage);
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // --- CORRECTION : Utilisation du constructeur de l'entité ---
                var newUser = new User(
                    id: BaseUtilities.GenerateUniqueNumber(36),
                    firstName: request.FirstName,
                    lastName: request.LastName,
                    email: request.Email,
                    passwordHash: passwordHash,
                    role: "User" // Le rôle est assigné ici, de manière contrôlée.
                );

                _userRepository.Add(newUser);
                await _uow.SaveAsync();

                var userDto = _mapper.Map<UserDto>(newUser);

                _logger.LogInformation("Successfully created user with ID {UserId} and Role {Role}.", newUser.Id, newUser.Role);
                return ServiceResponse<UserDto>.ReturnResultWith201(userDto, "User created successfully.");
            }
            catch (Exception ex)
            {
                // --- CORRECTION : Utilisation du template de message pour le logging ---
                _logger.LogError(ex, "An unexpected error occurred while creating the user for email {Email}.", request.Email);
                
                return ServiceResponse<UserDto>.Return500(ex);
            }
        }
    }
}
