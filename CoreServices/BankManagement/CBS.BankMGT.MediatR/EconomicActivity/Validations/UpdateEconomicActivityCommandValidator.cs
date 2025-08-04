using FluentValidation;
using CBS.BankMGT.MediatR.Commands;

namespace CBS.BankMGT.MediatR.Validations
{
    public class UpdateEconomicActivityCommandValidator : AbstractValidator<UpdateEconomicActivityCommand>
    {
        public UpdateEconomicActivityCommandValidator()
        {
            RuleFor(c => c.Description).NotEmpty().WithMessage("Please enter EconomicActivity decription.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Please enter EconomicActivity name.");
        }
    }
}
