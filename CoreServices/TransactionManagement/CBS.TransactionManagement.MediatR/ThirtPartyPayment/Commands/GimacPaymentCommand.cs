using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Data.Entity.ThirtPartyPayment;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Dto.Entity.ThirtPartyPayment;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.ThirtPartyPayment.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    using System.ComponentModel.DataAnnotations;

    public class GimacPaymentCommand : IRequest<ServiceResponse<GimacPaymentDto>>
    {
        [Required(ErrorMessage = "Account Number is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Account Number must be between 5 and 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Account Number can only contain letters and numbers.")]
        public string AccountNumber { get; set; }

        [StringLength(200, ErrorMessage = "Note can't be longer than 200 characters.")]
        public string Note { get; set; }

        [Required(ErrorMessage = "External Transaction Reference is required.")]
        [StringLength(20, ErrorMessage = "External Transaction Reference can't be longer than 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]*$", ErrorMessage = "External Transaction Reference can only contain letters, numbers, and hyphens.")]
        public string ExternalTransactionReference { get; set; }

        [Required(ErrorMessage = "Source Type is required.")]
        [StringLength(50, ErrorMessage = "Source Type can't be longer than 50 characters.")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Source Type can only contain letters.")]
        public string SourceType { get; set; }

        [Required(ErrorMessage = "Application Name is required.")]
        [StringLength(10, ErrorMessage = "Application Name can't be longer than 10 characters.")]
        public string ApplicationName { get; set; }

        [Required(ErrorMessage = "Application Code is required.")]
        [StringLength(3, ErrorMessage = "Application Code can't be longer than 3 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Application Code can only contain letters and numbers.")]
        public string ApplicationCode { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Item Type is required.")]
        [StringLength(50, ErrorMessage = "Item Type can't be longer than 50 characters.")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Item Type can only contain letters.")]
        public string ItemType { get; set; }
    }

}

