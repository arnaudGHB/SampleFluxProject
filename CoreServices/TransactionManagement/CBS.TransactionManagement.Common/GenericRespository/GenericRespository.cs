using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace CBS.TransactionManagement.Common.GenericRespository
{
    public class GenericRepository<TC, TContext> : IGenericRepository<TC>
        where TC : class
        where TContext : DbContext
    {
        protected readonly TContext Context;
        internal readonly DbSet<TC> DbSet;
        protected IUnitOfWork<TContext> _uow;
        protected GenericRepository(IUnitOfWork<TContext> uow)
        {
            Context = uow.Context;
            this._uow = uow;
            DbSet = Context.Set<TC>();
        }
        public IQueryable<TC> All => Context.Set<TC>();
        public void Add(TC entity)
        {
            Context.Add(entity);
        }
        public IQueryable<TC> GetAllNotDeleted()
        {
            var allEntities = FindBy(entity => !(entity is BaseEntity) || !((BaseEntity)(object)entity).IsDeleted);
            return allEntities;
        }


        public void AttachAssociatedEntities(TC entity)
        {
            // Get the DbContext instance and use reflection to find navigation properties
            var entry = Context.Entry(entity);

            // Loop through each navigation property
            foreach (var navigation in entry.Navigations)
            {
                if (navigation.CurrentValue is not null)
                {
                    if (navigation.Metadata.IsCollection)
                    {
                        // If the navigation property is a collection (e.g., ICollection<T>)
                        var collection = (IEnumerable<object>)navigation.CurrentValue;
                        foreach (var relatedEntity in collection)
                        {
                            AttachEntityIfDetached(relatedEntity);
                        }
                    }
                    else
                    {
                        // If the navigation property is a single entity
                        AttachEntityIfDetached(navigation.CurrentValue);
                    }
                }
            }
        }

        // Helper method to check if the entity is detached, and attach it if necessary
        private void AttachEntityIfDetached(object entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Context.Attach(entity);
            }
        }
        public IQueryable<TC> AllIncluding(params Expression<Func<TC, object>>[] includeProperties)
        {
            return GetAllIncluding(includeProperties);
        }

        public IQueryable<TC> FindByDateRange(
            Expression<Func<TC, DateTime>> dateProperty,
            DateTime startDate,
            DateTime endDate)
        {
            IQueryable<TC> query = DbSet.AsNoTracking();

            if (dateProperty != null)
            {
                var memberExpression = (MemberExpression)dateProperty.Body;
                var propertyInfo = (PropertyInfo)memberExpression.Member;

                var startDateParam = Expression.Parameter(typeof(TC));
                var endDateParam = Expression.Parameter(typeof(TC));

                var startDateCondition = Expression.GreaterThanOrEqual(
                    Expression.Property(startDateParam, propertyInfo),
                    Expression.Constant(startDate, typeof(DateTime))
                );

                var endDateCondition = Expression.LessThanOrEqual(
                    Expression.Property(endDateParam, propertyInfo),
                    Expression.Constant(endDate, typeof(DateTime))
                );

                var dateRangeCondition = Expression.AndAlso(startDateCondition, endDateCondition);

                var combinedPredicate = Expression.Lambda<Func<TC, bool>>(
                    dateRangeCondition,
                    startDateParam
                );

                query = query.Where(combinedPredicate);
            }

            return query;
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
        private object GetIdValue(TC entity)
        {
            var entityType = typeof(TC);
            var keyProperties = entityType.GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);

            if (keyProperties.Count() == 0)
                throw new Exception("No key");

            if (keyProperties.Count() > 1)
                throw new Exception("Composite keys not supported");

            var keyProperty = keyProperties.First();
            return keyProperty.GetValue(entity);
        }
        public void UpdateInCasecade(TC entity)
        {
            try
            {
                var IDS = GetIdValue(entity);
                var conflictingAccount = Context.Set<TC>().Find(GetIdValue(entity));
                if (conflictingAccount != null)
                {
                    // Merge changes from the new instance into the existing tracked instance
                    Context.Entry(conflictingAccount).CurrentValues.SetValues(entity);
                }
                else
                {
                    Context.Set<TC>().Attach(entity);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

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

       
    }
}
