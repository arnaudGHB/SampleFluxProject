using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class DepositNotifcationRepository : GenericRepository<DepositNotification, POSContext>, IDepositNotifcationRepository
    {
        public DepositNotifcationRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}