using CBS.SystemConfiguration.Common;

using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;

namespace CBS.SystemConfiguration.Repository
{
    public class DivisionRepository : GenericRepository<Division, SystemContext>, IDivisionRepository
    {
        public DivisionRepository(IUnitOfWork<SystemContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}