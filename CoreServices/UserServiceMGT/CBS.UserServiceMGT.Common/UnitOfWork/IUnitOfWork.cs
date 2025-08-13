// Source: Votre code source exact.
using Microsoft.EntityFrameworkCore;

namespace CBS.UserServiceMGT.Common
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        int Save();
        Task<int> SaveAsync();
        Task<int> SavingMigrationAsync(string branchId);
        TContext Context { get; }
    }
}