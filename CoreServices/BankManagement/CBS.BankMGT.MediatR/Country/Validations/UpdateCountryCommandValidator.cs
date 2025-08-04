using FluentValidation;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.MediatR.Validations
{
    public class UpdateCountryCommandValidator : AbstractValidator<UpdateCountryCommand>
    {
        public UpdateCountryCommandValidator()
        {
            RuleFor(c => c.Code).NotEmpty().WithMessage("Please enter country code.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Please enter country name.");
        }
    }
}
