using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Common.UnitOfWork
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
        //    using (var transaction = _context.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            SetModifiedInformation();
        //            var val = await _context.SaveChangesAsync();
        //            transaction.Commit();
        //            return val;
        //        }
        //        catch (Exception e)
        //        {

        //            Console.WriteLine(e.InnerException);
        //            transaction.Rollback();
        //            _logger.LogError(e, e.Message);
        //            throw new HttpRequestException($"Failed to persist to the database. Error: {e}");
        //        }
        //    }
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


        private static string GetChanges(EntityEntry entity)
        {
            var changes = new StringBuilder();
            foreach (var property in entity.OriginalValues.Properties)
            {
                var originalValue = entity.OriginalValues[property];
                var currentValue = entity.CurrentValues[property];
                if (!Equals(originalValue, currentValue))
                {
                    changes.AppendLine($"{property.Name}: From '{originalValue}' to '{currentValue}'");
                }
            }
            return changes.ToString();
        }
        public TContext Context => _context;
        public void Dispose()
        {
            _context.Dispose();
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
}
