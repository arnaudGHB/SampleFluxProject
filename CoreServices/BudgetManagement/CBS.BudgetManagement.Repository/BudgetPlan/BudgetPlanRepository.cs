using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;


namespace CBS.BudgetManagement.Repository
{
    public class BudgetPlanRepository : GenericRepository<BudgetPlan, BudgetManagementContext>, IBudgetPlanRepository
    {
        public BudgetPlanRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

}
