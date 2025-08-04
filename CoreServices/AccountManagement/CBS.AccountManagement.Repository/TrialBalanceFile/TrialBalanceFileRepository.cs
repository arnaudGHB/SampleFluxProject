using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;

using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Repository;

namespace CBS.AccountManagement.Repository
{
    public class TrialBalanceFileRepository : GenericRepository<TrialBalanceFile, POSContext>, ITrialBalanceFileRepository
    {
        public TrialBalanceFileRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}