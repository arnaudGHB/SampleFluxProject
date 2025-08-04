using CBS.SystemConfiguration.Common;

using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;

namespace CBS.SystemConfiguration.Repository
{
    public class SubdivisionRepository : GenericRepository<Subdivision, SystemContext>, ISubdivisionRepository
    {
        public SubdivisionRepository(IUnitOfWork<SystemContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}