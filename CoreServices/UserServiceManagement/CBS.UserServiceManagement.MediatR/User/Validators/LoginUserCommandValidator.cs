using FluentValidation;
using CBS.UserServiceManagement.MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis.")
                .EmailAddress().WithMessage("Un format d'email valide est requis.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis.");
        }
    }
}