using FluentValidation;
using CBS.UserServiceManagement.MediatR;

namespace CBS.UserServiceManagement.MediatR
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User ID is required.")
                .NotNull().WithMessage("User ID cannot be null.");
        }
    }
}
