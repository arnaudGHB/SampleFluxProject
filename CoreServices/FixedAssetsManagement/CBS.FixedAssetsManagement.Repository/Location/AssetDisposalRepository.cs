
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;

namespace CBS.FixedAssetsManagement.Repository
{
    public class LocationRepository : GenericRepository<Location, FixedAssetsContext> , ILocationRepository
    {
        public LocationRepository(IUnitOfWork<FixedAssetsContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}