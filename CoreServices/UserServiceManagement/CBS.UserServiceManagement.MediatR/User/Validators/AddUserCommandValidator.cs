// Source: Votre modèle AddUserCommandValidator.cs
using FluentValidation;
using CBS.UserServiceManagement.MediatR;



namespace CBS.UserServiceManagement.MediatR
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
    {
        public AddUserCommandValidator()
        {
            // Règle pour le prénom
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Le prénom est requis.")
                .MaximumLength(100).WithMessage("Le prénom ne peut pas dépasser 100 caractères.")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Le prénom ne peut contenir que des lettres, des espaces, des apostrophes et des tirets.");
               

            // Règle pour le nom de famille
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Le nom est requis.")
                .MaximumLength(100).WithMessage("Le nom ne peut pas dépasser 100 caractères.")
                .Matches(@"^[a-zA-Z\s\-']+$").WithMessage("Le nom ne peut contenir que des lettres, des espaces, des apostrophes et des tirets.");
                

            // Règle pour l'email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est requis.")
                .EmailAddress().WithMessage("Un format d'email valide est requis.")
                .MaximumLength(150).WithMessage("L'email ne peut pas dépasser 150 caractères.");

            // Règle pour le mot de passe
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Le mot de passe est requis.")
                .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères.")
                .Matches(@"[A-Z]").WithMessage("Le mot de passe doit contenir au moins une lettre majuscule.")
                .Matches(@"[a-z]").WithMessage("Le mot de passe doit contenir au moins une lettre minuscule.")
                .Matches(@"[0-9]").WithMessage("Le mot de passe doit contenir au moins un chiffre.");
            // On pourrait ajouter une règle pour les caractères spéciaux si nécessaire
            // .Matches(@"[\W_]").WithMessage("Le mot de passe doit contenir au moins un caractère spécial.");
        }
    }
}