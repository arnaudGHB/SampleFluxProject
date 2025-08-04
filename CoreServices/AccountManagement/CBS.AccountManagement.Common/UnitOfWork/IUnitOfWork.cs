using Microsoft.EntityFrameworkCore;

namespace CBS.AccountManagement.Common
{
    public interface IUnitOfWork<TContext>
        where TContext : DbContext
    {
        int Save();
        Task<int> SaveAsyncWithOutAffectingBranchId();
        Task<int> SaveAsync();
        Task<int> SavingMigrationAsync(string branchId);
        TContext Context { get; }
    }
}