using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class BudgetCategoryRepository : GenericRepository<BudgetCategory, POSContext>, IBudgetCategoryRepository
    {
        public BudgetCategoryRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}