using CBS.TransactionManagement.Data.Dto.OTP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.OTP.Commands
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
