using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.Repository
{
    public class BudgetItemRepository : GenericRepository<BudgetItem, BudgetManagementContext>, IBudgetItemRepository
    {
        public BudgetItemRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

}
