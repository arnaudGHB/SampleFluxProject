
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;

namespace CBS.FixedAssetsManagement.Repository
{
    public class AssetTransferRepository : GenericRepository<AssetTransfer, FixedAssetsContext> , IAssetTransferRepository
    {
        public AssetTransferRepository(IUnitOfWork<FixedAssetsContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}