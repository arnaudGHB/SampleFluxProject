using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Common.UnitOfWork
{
    public interface IDbContextFactory<TContext> where TContext : DbContext
    {
        TContext CreateDbContext();
    }
}