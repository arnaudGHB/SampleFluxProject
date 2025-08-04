using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class BudgetItemDetailRepository : GenericRepository<BudgetItemDetail, POSContext>, IBudgetItemDetailRepository
    {
        public BudgetItemDetailRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}