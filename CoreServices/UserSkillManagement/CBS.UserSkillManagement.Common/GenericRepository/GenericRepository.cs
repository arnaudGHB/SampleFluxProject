
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CBS.UserSkillManagement.Common
{
    public class GenericRepository<T, TContext> : IGenericRepository<T>
        where T : class
        where TContext : DbContext
    {
        protected readonly TContext Context;
        protected readonly DbSet<T> DbSet;
        protected readonly IUnitOfWork<TContext> _uow;

        public GenericRepository(IUnitOfWork<TContext> uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            Context = uow.Context;
            DbSet = Context.Set<T>();
        }
        
        public virtual async Task<T> GetByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await DbSet.AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null 
                ? await DbSet.CountAsync() 
                : await DbSet.CountAsync(predicate);
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        public virtual IQueryable<T> GetPaged(int pageNumber, int pageSize)
        {
            return DbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public virtual async Task SaveChangesAsync()
        {
            try
            {
                await _uow.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency conflicts here
                throw;
            }
            catch (DbUpdateException ex)
            {
                // Handle database update exceptions here
                throw;
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw;
            }
        }
    }
}
