using FluentValidation;
using CBS.UserServiceMGT.MediatR;

namespace CBS.UserServiceMGT.MediatR
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
