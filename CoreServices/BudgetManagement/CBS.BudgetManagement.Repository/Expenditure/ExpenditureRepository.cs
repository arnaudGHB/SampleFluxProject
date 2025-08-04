using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;

namespace CBS.BudgetManagement.Repository
{
    public class ExpenditureRepository : GenericRepository<Expenditure, BudgetManagementContext>, IExpenditureRepository
    {
        public ExpenditureRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
 
