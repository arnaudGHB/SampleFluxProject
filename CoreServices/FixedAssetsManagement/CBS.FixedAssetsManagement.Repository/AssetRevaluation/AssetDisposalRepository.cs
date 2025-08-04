
using CBS.FixedAssetsManagement.Common;
using CBS.FixedAssetsManagement.Data;
using CBS.FixedAssetsManagement.Domain;

namespace CBS.FixedAssetsManagement.Repository
{
    public class AssetRevaluationRepository : GenericRepository<AssetRevaluation, FixedAssetsContext> , IAssetRevaluationRepository
    {
        public AssetRevaluationRepository(IUnitOfWork<FixedAssetsContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}