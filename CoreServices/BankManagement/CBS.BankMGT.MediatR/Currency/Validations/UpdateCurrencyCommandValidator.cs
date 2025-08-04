using FluentValidation;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.MediatR.Validations
{
    public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
    {
        public UpdateCurrencyCommandValidator()
        {
            RuleFor(c => c.Code).NotEmpty().WithMessage("Please enter Currency code.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Please enter Currency name.");
            RuleFor(c => c.CountryID).NotEmpty().WithMessage("Please country id");
        }
    }
}
