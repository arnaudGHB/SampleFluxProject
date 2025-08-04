using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Notifications.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddOTPNotificationCommand : IRequest<ServiceResponse<OTPNotificationDto>>
    {
        public string LoanApplicationId { get; set; }
        public string CustomerId { get; set; }

    }

}
