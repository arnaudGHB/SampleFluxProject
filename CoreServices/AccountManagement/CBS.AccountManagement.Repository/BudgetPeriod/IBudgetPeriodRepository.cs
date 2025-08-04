using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
 

namespace CBS.AccountManagement.Repository
{
    public interface IBudgetPeriodRepository : IGenericRepository<BudgetPeriod>
    {
        List<BudgetPeriod> GenerateBudgetPeriods(int startYear, int endYear);
    }
}