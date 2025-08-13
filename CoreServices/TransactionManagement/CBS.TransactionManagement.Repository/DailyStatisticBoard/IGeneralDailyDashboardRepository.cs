using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.DailyStatisticBoard;

namespace CBS.TransactionManagement.Repository.DailyStatisticBoard
{
    public interface IGeneralDailyDashboardRepository : IGenericRepository<GeneralDailyDashboard>
    {
        Task<List<GeneralDailyDashboard>> GetDashboardDataForAllBranchesByDateAsync(DateTime date);
        Task<GeneralDailyDashboard> GetDashboardDataForBranchByDateAsync(string branchId, DateTime date);
        Task UpdateOrCreateDashboardAsync(CashOperation cashOperation,string reference=null);

    }
}
