using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class TransactionTrackerRepository : GenericRepository<TransactionTracker, POSContext>, ITransactionTrackerRepository
    {
        public TransactionTrackerRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}