using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.Notifications.Queries
{
    public class GetAllOTPNotificationQuery : IRequest<ServiceResponse<List<OTPNotificationDto>>>
    {
    }
}
