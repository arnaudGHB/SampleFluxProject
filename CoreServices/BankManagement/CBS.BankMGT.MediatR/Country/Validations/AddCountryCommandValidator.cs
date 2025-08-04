using FluentValidation;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.MediatR.Validations
{
    public class AddCountryCommandValidator : AbstractValidator<AddCountryCommand>
    {
        public AddCountryCommandValidator()
        {
            RuleFor(c => c.Code).NotEmpty().WithMessage("Please enter country code.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Please enter country name.");
            //RuleFor(c => c.Email).NotEmpty().WithMessage("Please enter email .");
            // RuleFor(c => c.Email).EmailAddress().WithMessage("Email in right format.");
        }
    }
}
