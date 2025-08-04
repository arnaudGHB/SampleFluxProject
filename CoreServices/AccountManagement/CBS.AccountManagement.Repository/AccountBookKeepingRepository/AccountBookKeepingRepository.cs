using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class AccountBookKeepingRepository : GenericRepository<AccountBookKeeping, POSContext>, IAccountBookKeepingRepository
    {
        public AccountBookKeepingRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}