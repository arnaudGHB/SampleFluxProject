using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class CashMovementTrackerRepository : GenericRepository<CashMovementTracker, POSContext>, ICashMovementTrackerRepository
    {
        public CashMovementTrackerRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}