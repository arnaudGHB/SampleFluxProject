using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class TellerDailyProvisionRepository : GenericRepository<TellerDailyProvision, POSContext>, ITellerDailyProvisionRepository
    {
        public TellerDailyProvisionRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}