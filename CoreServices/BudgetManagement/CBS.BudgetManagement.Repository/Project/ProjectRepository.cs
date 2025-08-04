using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;


namespace CBS.BudgetManagement.Repository
{
    public class ProjectRepository : GenericRepository<Project, BudgetManagementContext>, IProjectRepository
    {
        public ProjectRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

}
