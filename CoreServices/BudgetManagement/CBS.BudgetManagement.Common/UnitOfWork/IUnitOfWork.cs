using Microsoft.EntityFrameworkCore;

namespace CBS.BudgetManagement.Common
{
    public interface IUnitOfWork<TContext>
        where TContext : DbContext
    {
        int Save();

        Task<int> SaveAsync();
        Task<int> SavingMigrationAsync(string branchId);
        TContext Context { get; }
    }
}