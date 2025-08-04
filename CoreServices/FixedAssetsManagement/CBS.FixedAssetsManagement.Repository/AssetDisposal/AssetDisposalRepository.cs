
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;

namespace CBS.FixedAssetsManagement.Repository
{
    public class AssetDisposalRepository : GenericRepository<AssetDisposal, FixedAssetsContext> , IAssetDisposalRepository
    {
        public AssetDisposalRepository(IUnitOfWork<FixedAssetsContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}