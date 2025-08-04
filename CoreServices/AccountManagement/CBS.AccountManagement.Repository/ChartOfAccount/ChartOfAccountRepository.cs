using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class ChartOfAccountRepository : GenericRepository<ChartOfAccount, POSContext>, IChartOfAccountRepository
    {
        public ChartOfAccountRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}