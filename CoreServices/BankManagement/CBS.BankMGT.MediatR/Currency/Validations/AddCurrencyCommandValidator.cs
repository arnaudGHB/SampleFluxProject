using FluentValidation;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.MediatR.Validations
{
    public class AddCurrencyCommandValidator : AbstractValidator<AddCurrencyCommand>
    {
        public AddCurrencyCommandValidator()
        {
            RuleFor(c => c.Code).NotEmpty().WithMessage("Please enter Currency code.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Please enter Currency name.");
            RuleFor(c => c.CountryID).NotEmpty().WithMessage("Please select country.");
            //RuleFor(c => c.Email).NotEmpty().WithMessage("Please enter email .");
            // RuleFor(c => c.Email).EmailAddress().WithMessage("Email in right format.");
        }
    }
}
