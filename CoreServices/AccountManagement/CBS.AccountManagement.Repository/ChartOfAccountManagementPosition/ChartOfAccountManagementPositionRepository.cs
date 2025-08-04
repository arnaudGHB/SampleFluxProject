using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class ChartOfAccountManagementPositionRepository : GenericRepository<ChartOfAccountManagementPosition, POSContext>, IChartOfAccountManagementPositionRepository
    {
        public ChartOfAccountManagementPositionRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}