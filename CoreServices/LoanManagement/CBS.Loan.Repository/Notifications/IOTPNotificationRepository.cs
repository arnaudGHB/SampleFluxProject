using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.Notifications;

namespace CBS.NLoan.Repository.Notifications
{
    public interface IOTPNotificationRepository : IGenericRepository<OTPNotification>
    {
    }
}
