using CBS.CUSTOMER.DATA.Dto.OTP;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Command
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>

    public class GenerateMemberActivationOTPCommand : IRequest<ServiceResponse<TempOTPDto>>
    {
        [Required(ErrorMessage = "Customer ID is required.")]
        [StringLength(25, ErrorMessage = "Customer ID must not exceed 25 characters.")]
        public string CustomerId { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "The provided Phone Number is not valid.")]
        public string PhoneNumber { get; set; }
    }


}
