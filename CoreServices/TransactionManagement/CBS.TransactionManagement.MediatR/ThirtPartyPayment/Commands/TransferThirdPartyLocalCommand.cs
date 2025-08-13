using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.ThirtPartyPayment.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    using System.ComponentModel.DataAnnotations;

    public class TransferThirdPartyLocalCommand : IRequest<ServiceResponse<TransferThirdPartyDto>>
    {
        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Sender Account Number is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Sender Account Number must be between 5 and 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Sender Account Number can only contain letters and numbers.")]
        public string SenderAccountNumber { get; set; }

        [Required(ErrorMessage = "Receiver Account Number is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Receiver Account Number must be between 5 and 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Receiver Account Number can only contain letters and numbers.")]
        public string ReceiverAccountNumber { get; set; }

        [StringLength(250, ErrorMessage = "Note can't be longer than 250 characters.")]
        public string Note { get; set; }

        [Required(ErrorMessage = "Application Code is required.")]
        [StringLength(20, ErrorMessage = "Application Code can't be longer than 20 characters.")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Application Code can only contain letters and numbers.")]
        public string ApplicationCode { get; set; }

        [Required(ErrorMessage = "Source Type is required.")]
        [StringLength(50, ErrorMessage = "Source Type can't be longer than 50 characters.")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Source Type can only contain letters.")]
        public string SourceType { get; set; }

    }

}
