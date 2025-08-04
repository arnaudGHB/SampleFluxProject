using CBS.CUSTOMER.DATA.Dto.OTP;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.OTP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class VerifyTemporalOTPCommand : IRequest<ServiceResponse<TempVerificationOTPDto>>
    {
        public string UserId { get; set; }
        public string OtpCode { get; set; }

    }

}
