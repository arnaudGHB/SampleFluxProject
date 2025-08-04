using CBS.SystemConfiguration.Common;

using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;

namespace CBS.SystemConfiguration.Repository
{
    public class RegionRepository : GenericRepository<Region, SystemContext>, IRegionRepository
    {
        public RegionRepository(IUnitOfWork<SystemContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}