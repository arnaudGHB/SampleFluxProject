using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;


namespace CBS.BudgetManagement.Repository
{
    public class ProjectBudgetRepository : GenericRepository<ProjectBudget, BudgetManagementContext>, IProjectBudgetRepository
    {
        public ProjectBudgetRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

}
