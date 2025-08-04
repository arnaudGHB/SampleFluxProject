using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using FluentValidation;

namespace CBS.BankMGT.MediatR.Country.Validations
{
    public class AddCustomerCommandValidator : AbstractValidator<AddCustomerCommand>
    {
        public AddCustomerCommandValidator()
        {
            RuleFor(c => c.FirstName)
         .NotEmpty().WithMessage("First name cannot be null or empty ")
         .MaximumLength(50).WithMessage("First name must not exceed 50 characters.")
         .Matches("^[A-Za-z ]+$").WithMessage("Please enter a valid first name containing only letters.");

            RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("Last name cannot be null or empty ")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.")
                .Matches("^[A-Za-z ]+$").WithMessage("Please enter a valid last name containing only letters.");

            RuleFor(c => c.DateOfBirth)
                .Must(BeAValidDate)
                .Must(BeAValidAge).WithMessage("Please enter a valid date of birth. Or Age must be greater 20Years");

            RuleFor(c => c.Gender)
                .NotEmpty().WithMessage("Please select a valid gender.");

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter a valid email address.")
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

            RuleFor(c => c.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Must(BeAValidCameroonMobile).WithMessage("Please enter a valid Cameroon mobile number."); 
            


            RuleFor(c => c.IDNumber)
                .NotEmpty().WithMessage("ID number is required.")
                .MaximumLength(20).WithMessage("ID number must not exceed 20 characters.");

            

          

        }




        private bool BeAValidDate(DateTime date)
        {
            return date <= DateTime.Now;
        }   
        
        private bool BeAValidAge(DateTime date)
        {
            return  (DateTime.Now.Year-date.Year)>20;
        }

        private bool BeAValidCameroonMobile(string phone)
        {
            // Assuming Cameroon mobile numbers start with prefixes "6" or "2"
            return phone.StartsWith("6") || phone.StartsWith("2");
        }
       
    }
}
