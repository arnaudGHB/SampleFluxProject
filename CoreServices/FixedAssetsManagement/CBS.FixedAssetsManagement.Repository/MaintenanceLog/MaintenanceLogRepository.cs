
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;

namespace CBS.FixedAssetsManagement.Repository
{
    public class MaintenanceLogRepository : GenericRepository<MaintenanceLog, FixedAssetsContext> , IMaintenanceLogRepository
    {
        public MaintenanceLogRepository(IUnitOfWork<FixedAssetsContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}