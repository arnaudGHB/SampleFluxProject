using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class AccountingRuleRepository : GenericRepository<AccountingRule, POSContext>, IAccountingRuleRepository
    {
        public AccountingRuleRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}