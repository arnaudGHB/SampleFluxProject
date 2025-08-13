using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CBS.UserSkillManagement.Common
{
    public interface IUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        TContext Context { get; }
        IGenericRepository<T> GetRepository<T>() where T : class;
        Task<int> SaveChangesAsync();
        int SaveChanges();
    }
}
