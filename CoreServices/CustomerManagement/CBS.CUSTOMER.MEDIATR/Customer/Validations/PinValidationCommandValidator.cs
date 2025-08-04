using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using FluentValidation;

namespace CBS.BankMGT.MediatR.Validations
{
    public class PinValidationCommandValidator : AbstractValidator<PinValidationCommand>
    {
        public PinValidationCommandValidator()
        {
            RuleFor(c => c.Telephone)
         .NotEmpty().WithMessage("Telephone is required.")
         .Matches("^[0-9]+$").WithMessage("Please enter a valid first name containing only digits.")
          .Must(BeAValidCameroonMobile).WithMessage("Please enter a valid Cameroon mobile number.");

            RuleFor(c => c.Pin)
                .NotEmpty().WithMessage("Pin is required.")
                .MaximumLength(5).WithMessage("Pin must not exceed 5 characters.")
                .Matches("^[0-9]+$").WithMessage("Please enter a valid Pin containing only digits.");

         
          

        }
     

        private bool BeAValidCameroonMobile(string phone)
        {
            // Assuming Cameroon mobile numbers start with prefixes "6" or "2"
            return phone.StartsWith("6") || phone.StartsWith("2");
        }
       
    }
}
