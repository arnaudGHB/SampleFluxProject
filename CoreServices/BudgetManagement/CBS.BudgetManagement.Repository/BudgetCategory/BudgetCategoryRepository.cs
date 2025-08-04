using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;


namespace CBS.BudgetManagement.Repository
{
    public class BudgetCategoryRepository : GenericRepository<BudgetCategory, BudgetManagementContext>, IBudgetCategoryRepository
    {
        public BudgetCategoryRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

}
