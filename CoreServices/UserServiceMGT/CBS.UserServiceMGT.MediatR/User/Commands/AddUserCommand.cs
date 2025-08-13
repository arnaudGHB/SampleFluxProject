// Fichier : CBS.UserServiceMGT.MediatR/AddUserCommand.cs

using CBS.UserServiceMGT.Data; // Assurez-vous d'avoir ce using pour UserDto
using CBS.UserServiceMGT.Helper;
using MediatR;

namespace CBS.UserServiceMGT.MediatR
{
    // Le type de retour est maintenant ServiceResponse<UserDto> et non plus Guid.
    public class AddUserCommand : IRequest<ServiceResponse<UserDto>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public string Role { get; set; } // AJOUTER CETTE LIGNE
    }
}