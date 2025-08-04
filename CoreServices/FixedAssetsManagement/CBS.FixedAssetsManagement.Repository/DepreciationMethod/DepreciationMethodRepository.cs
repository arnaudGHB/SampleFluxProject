
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;

namespace CBS.FixedAssetsManagement.Repository
{
    public class DepreciationMethodRepository : GenericRepository<DepreciationMethod, FixedAssetsContext> , IDepreciationMethodRepository
    {
        public DepreciationMethodRepository(IUnitOfWork<FixedAssetsContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}