using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Common.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext>
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly ILogger<UnitOfWork<TContext>> _logger;
        private readonly UserInfoToken _userInfoToken;
        public UnitOfWork(
            TContext context,
            ILogger<UnitOfWork<TContext>> logger,
            UserInfoToken userInfoToken)
        {
            _context = context;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }
        public int Save()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    SetModifiedInformation();
                    var retValu = _context.SaveChanges();
                    transaction.Commit();
                    return retValu;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e, e.Message);
                    return 0;
                }
            }
        }
        //public async Task<int> SaveAsync()
        //{
        //    var strategy = _context.Database.CreateExecutionStrategy();

        //    return await strategy.ExecuteAsync(async () =>
        //    {
        //        using (var transaction = _context.Database.BeginTransaction())
        //        {
        //            try
        //            {
        //                SetModifiedInformation();

        //                // Nullify virtual navigation properties
        //                foreach (var entry in _context.ChangeTracker.Entries())
        //                {
        //                    foreach (var navigationEntry in entry.Navigations)
        //                    {
        //                        if (navigationEntry.IsLoaded && !navigationEntry.Metadata.IsCollection() && !navigationEntry.Metadata.IsOwned())
        //                        {
        //                            navigationEntry.CurrentValue = null;
        //                        }
        //                    }
        //                }

        //                var val = await _context.SaveChangesAsync();
        //                await transaction.CommitAsync();
        //                return val;
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.InnerException);
        //                await transaction.RollbackAsync();
        //                _logger.LogError(e, e.Message);
        //                throw new HttpRequestException($"Failed to persist to the database. Error: {e}");
        //            }
        //        }
        //    });
        //}

        public async Task<int> SaveAsync()
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        SetModifiedInformation();
                        var val = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return val;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException);
                        await transaction.RollbackAsync();
                        _logger.LogError(e, e.Message);
                        throw new HttpRequestException($"Failed to persist to the database. Error: {e}");
                    }
                }
            });
        }

        public async Task<int> SaveAsync(UserInfoToken userInfoToken)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        SetModifiedInformation(userInfoToken);
                        var val = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return val;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException);
                        await transaction.RollbackAsync();
                        _logger.LogError(e, e.Message);
                        throw new HttpRequestException($"Failed to persist to the database. Error: {e}");
                    }
                }
            });
        }

        

        public TContext Context => _context;
        public void Dispose()
        {
            _context.Dispose();
        }

        private void SetModifiedInformation(UserInfoToken userInfoToken)
        {
            foreach (var entry in Context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.CreatedBy = userInfoToken.Id;
                    entry.Entity.ModifiedBy = userInfoToken.Id;
                    entry.Entity.ModifiedDate = new DateTime();
                    entry.Entity.DeletedDate = new DateTime();
                    entry.Entity.ModifiedDate = new DateTime();
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity.IsDeleted)
                    {
                        entry.Entity.DeletedBy = userInfoToken.Id;
                        entry.Entity.DeletedDate = DateTime.Now;
                    }
                    else
                    {
                        entry.Entity.ModifiedBy = userInfoToken.Id;
                        entry.Entity.ModifiedDate = DateTime.Now;
                    }
                }
            }
        }

        private void SetModifiedInformation()
        {
            foreach (var entry in Context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.CreatedBy = _userInfoToken.Id;
                    entry.Entity.ModifiedBy = _userInfoToken.Id;
                    entry.Entity.ModifiedDate = new DateTime();
                    entry.Entity.DeletedDate = new DateTime();
                    entry.Entity.ModifiedDate = new DateTime();
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity.IsDeleted)
                    {
                        entry.Entity.DeletedBy = _userInfoToken.Id;
                        entry.Entity.DeletedDate = DateTime.Now;
                    }
                    else
                    {
                        entry.Entity.ModifiedBy = _userInfoToken.Id;
                        entry.Entity.ModifiedDate = DateTime.Now;
                    }
                }
            }
        }
    }

    public class DbContextFactory<TContext> : IDbContextFactory<TContext> where TContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TContext CreateDbContext()
        {
            return _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<TContext>();
        }
    }

}
