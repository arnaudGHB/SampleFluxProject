using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.ThirtPartyPayment.Commands
{
    using System.ComponentModel.DataAnnotations;

    public class AddThirdPartyCashInRequestCommand : IRequest<ServiceResponse<TransactionThirdPartyDto>>
    {
        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Account Number must be between 5 and 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Account Number can only contain letters and numbers.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "External Reference is required.")]
        [StringLength(50, ErrorMessage = "External Reference can't be longer than 50 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]*$", ErrorMessage = "External Reference can only contain letters, numbers, and hyphens.")]
        public string ExternalReference { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [GreaterThanZero(ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Charge must be a positive number.")]
        public decimal Charge { get; set; }

        [Required(ErrorMessage = "Application Code is required.")]
        [StringLength(3, ErrorMessage = "Application Code cannot exceed 3 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Application Code can only contain letters and numbers.")]
        public string ApplicationCode { get; set; }
    }

    public class GreaterThanZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is decimal amount)
            {
                return amount > 0;
            }
            return false; // If value is not decimal or null
        }
    }

}
