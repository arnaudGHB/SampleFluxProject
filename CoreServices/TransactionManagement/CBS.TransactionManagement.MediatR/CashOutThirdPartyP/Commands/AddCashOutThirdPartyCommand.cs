using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.CashOutThirdPartyP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.CashOutThirdPartyP.Commands
{
    /// <summary>
    /// Represents a command to add a new DepositLimit.
    /// </summary>
    using System.ComponentModel.DataAnnotations;

    public class AddCashOutThirdPartyCommand : IRequest<ServiceResponse<CashOutThirdPartyDto>>
    {
        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Account Number must be between 5 and 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Account Number can only contain letters and numbers.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [GreaterThanZero(ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "PIN is required.")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "PIN must be between 4 and 6 characters.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "PIN can only contain numbers.")]
        public string Pin { get; set; }

        [Required(ErrorMessage = "Application Code is required.")]
        [StringLength(3, ErrorMessage = "Application Code cannot exceed 3 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Application Code can only contain letters and numbers.")]
        public string ApplicationCode { get; set; }

        [Required(ErrorMessage = "Source Type is required.")]
        [StringLength(50, ErrorMessage = "Source Type cannot exceed 50 characters.")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Source Type can only contain letters.")]
        public string SourceType { get; set; }

        [Required(ErrorMessage = "External Transaction Reference is required.")]
        [StringLength(20, ErrorMessage = "External Transaction Reference cannot exceed 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]*$", ErrorMessage = "External Transaction Reference can only contain letters, numbers, and hyphens.")]
        public string ExternalTransactionReference { get; set; }
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
