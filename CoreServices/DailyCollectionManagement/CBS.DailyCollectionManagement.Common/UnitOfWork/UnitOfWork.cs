using CBS.DailyCollectionManagement.Data.Dto;
using CBS.DailyCollectionManagement.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;

namespace CBS.DailyCollectionManagement.Common
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
        public async Task<int> SaveAsync()
        {
             
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        SetModifiedInformation();
                        var val = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return val;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var conflictingEntry = ex.Entries.Single(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);

                        // Get the entity that caused the conflict 67
                        var conflictingEntity = conflictingEntry.Entity;

                        return 1;
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        string values = GetInnerExceptionMessages(ex);
                         throw new DbUpdateException($"Failed to persist to the database. Error: {values}");
                    }             
                    catch (Exception ex)
                    {
                        string message = GetInnerExceptionMessages(ex);
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        throw new HttpRequestException($"Failed to persist to the database. Error: {ex.Message}");
                    }
                }
            });
        }

        public async Task<int> SaveAsyncWithOutAffectingBranchId()
        {

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        SetModifiedInformationWithoutAffectingBranchId();
                        var val = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return val;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var conflictingEntry = ex.Entries.Single(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);

                        // Get the entity that caused the conflict
                        var conflictingEntity = conflictingEntry.Entity;

                        return 1;
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        string values = GetInnerExceptionMessages(ex);
                        throw new DbUpdateException($"Failed to persist to the database. Error: {values}");
                    }

                    catch (Exception ex)
                    {
                        string message = GetInnerExceptionMessages(ex);
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        throw new HttpRequestException($"Failed to persist to the database. Error: {ex.Message}");
                    }
                }
            });
        }
        public async Task<int> SavingMigrationAsync(string b)
        {

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        SetModifiedInformationForMigration(  b);
                        var val = await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return val;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        var conflictingEntry = ex.Entries.Single(e => e.State == EntityState.Modified || e.State == EntityState.Deleted);

                        // Get the entity that caused the conflict
                        var conflictingEntity = conflictingEntry.Entity;

                        return 1;
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        string values = GetInnerExceptionMessages(ex);
                        throw new DbUpdateException($"Failed to persist to the database. Error: {values}");
                    }

                    catch (Exception ex)
                    {
                        string message = GetInnerExceptionMessages(ex);
                        _logger.LogError(ex, ex.Message);
                        await transaction.RollbackAsync();
                        throw new HttpRequestException($"Failed to persist to the database. Error: {ex.Message}");
                    }
                }
            });
        }
        public string GetInnerExceptionMessages(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            var innerMessage = "";

            if (exception.InnerException != null)
            {
                innerMessage = "\n" + GetInnerExceptionMessages(exception.InnerException);
            }

            return exception.Message + innerMessage;
        }
        public TContext Context => _context;

        public void Dispose()
        {
            _context.Dispose();
        }


        private void SetModifiedInformationWithoutAffectingBranchId()
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
                    entry.Entity.DeletedBy = null;
                    entry.Entity.IsDeleted = false;
                    // Check if the entity is of type User
 

                    


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
        private void SetModifiedInformation()
        {
            foreach (var entry in Context.ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                 
                    entry.Entity.CreatedDate = DateTime.Now;
                    entry.Entity.CreatedBy = _userInfoToken.Id;
                    entry.Entity.ModifiedBy = _userInfoToken.Id;
                    entry.Entity.FullName = _userInfoToken.FullName;
                    entry.Entity.ModifiedDate = new DateTime();
                    entry.Entity.DeletedDate = new DateTime();
                    entry.Entity.ModifiedDate = new DateTime();
                    entry.Entity.DeletedBy = null;
                    entry.Entity.IsDeleted = false;
                    // Check if the entity is of type User
                  
                     

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

        private void SetModifiedInformationForMigration(string b)
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
                    entry.Entity.DeletedBy = null;
                    entry.Entity.IsDeleted = false;
                
                 

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