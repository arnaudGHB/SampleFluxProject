using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Commands
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>

    public class AddRemittanceCommand : IRequest<ServiceResponse<RemittanceDto>>
    {
        [Required(ErrorMessage = "The Account Number field is mandatory and cannot be empty.")]
        [StringLength(20, MinimumLength = 16, ErrorMessage = "The Account Number must be between 16 and 20 characters in length.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "The Source Branch Code field is mandatory and cannot be empty.")]
        public string SourceBranchCode { get; set; }

        [Required(ErrorMessage = "The Source Branch Id field is mandatory and must be provided.")]
        public string SourceBranchId { get; set; }

        [Required(ErrorMessage = "The Source Branch Name field is mandatory and cannot be empty.")]
        public string SourceBranchName { get; set; }

        [StringLength(10, MinimumLength = 4, ErrorMessage = "The Sender Secret Code must be between 4 and 10 characters in length.")]
        public string? SenderSecreteCode { get; set; }

        [Required(ErrorMessage = "The Sender Name field is mandatory and cannot be empty.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "The Sender Name must be between 3 and 30 characters in length.")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "The Sender CNI field is mandatory and cannot be empty.")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "The Sender CNI must be between 9 and 20 characters in length.")]
        public string SenderCNI { get; set; }

        [Required(ErrorMessage = "The Sender Phone Number field is mandatory and cannot be empty.")]
        [Phone(ErrorMessage = "The Sender Phone Number provided is not in a valid format.")]
        public string SenderPhoneNumber { get; set; }

        [Required(ErrorMessage = "The Sender Address is mandatory and cannot be empty.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "The Sender Address must be between 5 and 100 characters in length.")]
        public string SenderAddress { get; set; }

        [Required(ErrorMessage = "The Receiver Name field is mandatory and cannot be empty.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "The Receiver Name must be between 3 and 30 characters in length.")]
        public string ReceiverName { get; set; }

        [StringLength(20, MinimumLength = 5, ErrorMessage = "The Receiver CNI must be between 5 and 20 characters in length.")]
        public string? ReceiverCNI { get; set; }

        [Required(ErrorMessage = "The Receiver Address field is mandatory and cannot be empty.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "The Receiver Address must be between 5 and 100 characters in length.")]
        public string ReceiverAddress { get; set; }

        [Required(ErrorMessage = "The Receiver Phone Number field is mandatory and cannot be empty.")]
        [Phone(ErrorMessage = "The Receiver Phone Number provided is not in a valid format.")]
        public string ReceiverPhoneNumber { get; set; }

        [Required(ErrorMessage = "The Amount field is mandatory and must be provided.")]
        [Range(1, double.MaxValue, ErrorMessage = "The Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "The Fee field is mandatory and must be provided.")]
        [Range(0, double.MaxValue, ErrorMessage = "The Fee cannot be a negative value.")]
        public decimal Fee { get; set; }

        [Required(ErrorMessage = "The Remittance Type field is mandatory and cannot be empty.")]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "The Remittance Type must be between 3 and 15 characters in length.")]
        public string RemittanceType { get; set; }

        [StringLength(100, MinimumLength = 10, ErrorMessage = "The Sender Note must be between 10 and 100 characters in length.")]
        public string? SenderNote { get; set; }
        public string ReceiverLanguage { get; set; }
        public decimal InitailAmount { get; set; }
        [Required(ErrorMessage = "Charge Type is required.")]
        [RegularExpression("Inclussive|Exclussive", ErrorMessage = "Charge Type must be either 'Inclussive' or 'Exclussive'.")]
        public string ChargeType { get; set; }

        [Required(ErrorMessage = "SMS preference must be specified.")]
        public bool SendSMSTotReceiver { get; set; }

        [Required(ErrorMessage = "Date of Issue is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Date of Issue format.")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date of Issue must be in the format yyyy-MM-dd.")]
        public string DateOfIssue { get; set; }

        [Required(ErrorMessage = "Expiration Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Expiration Date format.")]
        [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Expiration Date must be in the format yyyy-MM-dd.")]
        public string ExpirationDate { get; set; }

        public string ExternalReference { get; set; }


        [Required(ErrorMessage = "Place of Issue is required.")]
        [StringLength(100, ErrorMessage = "Place of Issue must not exceed 100 characters.")]
        public string PlaceOfIssue { get; set; }

        [Required(ErrorMessage = "Trnsfer Type is required")]

        public string TransferType { get; set; } //Incoming & Out_Going Transfer
        [Required(ErrorMessage = "Transfer source is required")]

        public string TransferSource { get; set; }//International_Remittance Or Local_Remittance
        public DateTime? InternationalTransfterDate { get; set; }

    }

}
