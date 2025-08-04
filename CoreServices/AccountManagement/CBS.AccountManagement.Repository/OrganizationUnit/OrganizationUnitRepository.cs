using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
 
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class OrganizationalUnitRepository : GenericRepository<OrganizationalUnit, POSContext>, IOrganizationalUnitRepository
    {
        public OrganizationalUnitRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}