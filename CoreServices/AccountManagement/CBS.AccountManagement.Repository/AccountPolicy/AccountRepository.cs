using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class AccountPolicyRepository : GenericRepository<AccountPolicy, POSContext>, IAccountPolicyRepository
    {
        public AccountPolicyRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}