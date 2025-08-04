using CBS.SystemConfiguration.Common;

using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;

namespace CBS.SystemConfiguration.Repository
{
    public class TownRepository : GenericRepository<Town, SystemContext>, ITownRepository
    {
        public TownRepository(IUnitOfWork<SystemContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}