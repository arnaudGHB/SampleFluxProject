using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Notifications.Commands
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
