using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
 
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class CashMovementTrackingConfigurationRepository : GenericRepository<CashMovementTrackingConfiguration, POSContext>, ICashMovementTrackingConfigurationRepository
    {
        public CashMovementTrackingConfigurationRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}