using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class AccountCategoryRepository : GenericRepository<AccountCategory, POSContext>, IAccountCategoryRepository
    {
        public AccountCategoryRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}