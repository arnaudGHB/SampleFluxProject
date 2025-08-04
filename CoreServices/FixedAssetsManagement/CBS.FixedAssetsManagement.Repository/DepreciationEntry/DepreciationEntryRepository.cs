
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;

namespace CBS.FixedAssetsManagement.Repository
{
    public class DepreciationEntryRepository : GenericRepository<DepreciationEntry, FixedAssetsContext> , IDepreciationEntryRepository
    {
        public DepreciationEntryRepository(IUnitOfWork<FixedAssetsContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}