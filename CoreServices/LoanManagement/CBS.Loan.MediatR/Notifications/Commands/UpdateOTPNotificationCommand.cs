using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Notifications.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateOTPNotificationCommand : IRequest<ServiceResponse<OTPNotificationDto>>
    {
        public string Id { get; set; }
        public string Status { get; set; }
    }

}
