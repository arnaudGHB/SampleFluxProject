using CBS.BudgetManagement.Common;
using CBS.BudgetManagement.Data;
using CBS.BudgetManagement.Domain;


namespace CBS.BudgetManagement.Repository
{
    public class FiscalYearRepository : GenericRepository<FiscalYear, BudgetManagementContext>, IFiscalYearRepository
    {
        public FiscalYearRepository(IUnitOfWork<BudgetManagementContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
