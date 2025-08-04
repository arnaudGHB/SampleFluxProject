using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Notifications.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class GenerateTemporalOTPCommand : IRequest<ServiceResponse<TempOTPDto>>
    {
        public string Id { get; set; }
        public bool IsVerify { get; set; } = true;

    }
//  {
//  "userId": "650535634",
//  "otpCode": "4500"
//}
}
