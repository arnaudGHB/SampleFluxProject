using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;

using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class TrailBalanceUploudRepository : GenericRepository<TrailBalanceUploud, POSContext>, ITrailBalanceUploudRepository
    {
        public TrailBalanceUploudRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}