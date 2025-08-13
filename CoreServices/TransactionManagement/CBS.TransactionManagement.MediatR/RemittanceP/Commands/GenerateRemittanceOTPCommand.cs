using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Data.Dto.User;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Commands
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>

    public class GenerateRemittanceOTPCommand : IRequest<ServiceResponse<TempOTPDto>>
    {
        [Required(ErrorMessage = "Remittance Reference is needed.")]
        [StringLength(25, ErrorMessage = "The Account Number must be max of 25 characters in length.")]
        public string RemittanceReference { get; set; }

   
        [Required(ErrorMessage = "The Receiver Phone Number field is mandatory and cannot be empty.")]
        [Phone(ErrorMessage = "The Receiver Phone Number provided is not in a valid format.")]
        public string ReceiverPhoneNumber { get; set; }
    }

}
