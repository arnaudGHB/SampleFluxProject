using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.Notifications;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.Notification
{

    public class LoanNotificationRepository : GenericRepository<LoanNotificationSetting, LoanContext>, ILoanNotificationRepository
    {
        public LoanNotificationRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
