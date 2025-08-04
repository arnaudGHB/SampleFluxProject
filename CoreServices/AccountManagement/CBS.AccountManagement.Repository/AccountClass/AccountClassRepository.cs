using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class AccountClassRepository : GenericRepository<AccountClass, POSContext>, IAccountClassRepository
    {
        public AccountClassRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}