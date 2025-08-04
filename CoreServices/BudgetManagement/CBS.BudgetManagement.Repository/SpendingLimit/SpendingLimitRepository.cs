using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;


namespace CBS.BudgetManagement.Repository
{
    public class SpendingLimitRepository : GenericRepository<SpendingLimit, BudgetManagementContext>, ISpendingLimitRepository
    {
        public SpendingLimitRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
