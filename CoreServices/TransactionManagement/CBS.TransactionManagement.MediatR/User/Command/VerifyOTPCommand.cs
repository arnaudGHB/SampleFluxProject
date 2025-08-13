using CBS.TransactionManagement.Data.Dto.User;
using CBS.TransactionManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.MediatR.User.Command
{
    using System.ComponentModel.DataAnnotations;

    public class VerifyOTPCommand : IRequest<ServiceResponse<OTPDto>>
    {
        [Required(ErrorMessage = "User ID is required.")]
        [StringLength(50, ErrorMessage = "User ID cannot exceed 50 characters.")]
        [RegularExpression("^[a-zA-Z0-9-]*$", ErrorMessage = "User ID can only contain letters, numbers, and hyphens.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "OTP Code is required.")]
        [RegularExpression("^[0-9]{4}$", ErrorMessage = "OTP Code must be a 4-digit number.")]
        public string OtpCode { get; set; }
    }

}
