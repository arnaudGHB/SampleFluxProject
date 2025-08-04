using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.Repository
{

    public class BudgetAdjustmentRepository : GenericRepository<BudgetAdjustment, BudgetManagementContext>, IBudgetAdjustmentRepository
    {
        public BudgetAdjustmentRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

}
