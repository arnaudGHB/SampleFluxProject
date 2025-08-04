using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.Notifications;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Repository.Notifications;

namespace CBS.NLoan.Repository.Notification
{

    public class OTPNotificationRepository : GenericRepository<OTPNotification, LoanContext>, IOTPNotificationRepository
    {
        public OTPNotificationRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
