using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class AccountingRuleEntryRepository : GenericRepository<AccountingRuleEntry, POSContext>, IAccountingRuleEntryRepository
    {
        public AccountingRuleEntryRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}