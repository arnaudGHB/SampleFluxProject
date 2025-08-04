using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA;

namespace CBS.CUSTOMER.COMMON.GenericRespository
{
    public class GenericRepository<TC, TContext> : IGenericRepository<TC>
        where TC : class
        where TContext : DbContext
    {
        protected readonly TContext Context;
        internal readonly DbSet<TC> DbSet;
        protected IUnitOfWork<TContext> _uow;
        protected GenericRepository(IUnitOfWork<TContext> uow
            )
        {
            Context = uow.Context;
            _uow = uow;
            DbSet = Context.Set<TC>();
        }
        public IQueryable<TC> All => Context.Set<TC>();
        public void Add(TC entity)
        {
            Context.Add(entity);
        }

        public IQueryable<TC> AllIncluding(params Expression<Func<TC, object>>[] includeProperties)
        {
            return GetAllIncluding(includeProperties);
        }


        public IQueryable<TC> FindByInclude(Expression<Func<TC, bool>> predicate, params Expression<Func<TC, object>>[] includeProperties)
        {
            var query = GetAllIncluding(includeProperties);
            return query.Where(predicate);
        }

        public IQueryable<TC> FindBy(Expression<Func<TC, bool>> predicate)
        {
            IQueryable<TC> queryable = DbSet.AsNoTracking();
            return queryable.Where(predicate);
        }
        public async Task<bool> AnyAsync(Expression<Func<TC, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        private IQueryable<TC> GetAllIncluding(params Expression<Func<TC, object>>[] includeProperties)
        {
            IQueryable<TC> queryable = DbSet.AsNoTracking();

            return includeProperties.Aggregate
              (queryable, (current, includeProperty) => current.Include(includeProperty));
        }
        public TC Find(string id)
        {
            return Context.Set<TC>().Find(id);
        }

        public async Task<TC> FindAsync(string id)
        {
            return await Context.Set<TC>().FindAsync(id);
        }

        public virtual void Update(TC entity)
        {
            Context.Update(entity);
        }
        public virtual void UpdateRange(List<TC> entities)
        {
            Context.UpdateRange(entities);
        }

        public void RemoveRange(IEnumerable<TC> lstEntities)
        {
            Context.Set<TC>().RemoveRange(lstEntities);
        }

        public void AddRange(IEnumerable<TC> lstEntities)
        {
            Context.Set<TC>().AddRange(lstEntities);
        }

        public void InsertUpdateGraph(TC entity)
        {
            Context.Set<TC>().Add(entity);
            //Context.ApplyStateChanges(user);
        }
        public virtual void Delete(string id)
        {
            var entity = Context.Set<TC>().Find(id) as BaseEntity;
            if (entity != null)
            {
                entity.IsDeleted = true;
                Context.Update(entity);
            }
        }
        public virtual void Delete(TC entityData)
        {
            var entity = entityData as BaseEntity;
            entity.IsDeleted = true;
            Context.Update(entity);
        }
        public virtual void Remove(TC entity)
        {
            Context.Remove(entity);
        }
        public void Dispose()
        {
            Context.Dispose();
        }

        /*public virtual async Task<IEnumerable<TC>> WildcardSearchCollection(string filed_Name, string value)
        {
            var filter = Builders<TC>.Filter.Regex(filed_Name, new RegularExpression($".*{value}.*"));
            var data = await (await DbSet.FindAsync<TC>(filter).ConfigureAwait(false)).ToListAsync();
            return data;
        }*/
    }
}
