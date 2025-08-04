using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.Notifications;

namespace CBS.NLoan.Repository.Notification
{
    public interface ILoanNotificationRepository : IGenericRepository<LoanNotificationSetting>
    {
    }
}
