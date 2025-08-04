// Fichier : CBS.UserServiceManagement.MediatR/LoginUserCommand.cs

using CBS.UserServiceManagement.Helper;
using MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    // Le type de retour est standardisé
    public class LoginUserCommand : IRequest<ServiceResponse<LoginResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    // Le DTO de réponse est défini ici pour plus de clarté
    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; } // Gardé pour une évolution future
    }
}