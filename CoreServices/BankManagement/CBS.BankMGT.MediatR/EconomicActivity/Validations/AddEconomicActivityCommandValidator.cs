using FluentValidation;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.MediatR.Validations
{
    public class AddEconomicActivityCommandValidator : AbstractValidator<AddEconomicActivityCommand>
    {
        public AddEconomicActivityCommandValidator()
        {
            RuleFor(c => c.Description).NotEmpty().WithMessage("Please enter EconomicActivity description.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Please enter EconomicActivity name.");
        }
    }
}
