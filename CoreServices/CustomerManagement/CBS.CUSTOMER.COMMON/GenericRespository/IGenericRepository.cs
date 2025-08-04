using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.COMMON.GenericRespository
{
    public interface IGenericRepository<TC>
        where TC : class
    {
        IQueryable<TC> All { get; }
        IQueryable<TC> AllIncluding(params Expression<Func<TC, object>>[] includeProperties);
        //Task<IEnumerable<TC>> AllIncludingAsync(params Expression<Func<TC, object>>[] includeProperties);
        IQueryable<TC> FindByInclude(Expression<Func<TC, bool>> predicate, params Expression<Func<TC, object>>[] includeProperties);
        //Task<IEnumerable<TC>> FindByIncludeAsync(Expression<Func<TC, bool>> predicate, params Expression<Func<TC, object>>[] includeProperties);
        IQueryable<TC> FindBy(Expression<Func<TC, bool>> predicate);
        //Task<IEnumerable<TC>> FindByAsync(Expression<Func<TC, bool>> predicate);
        //IQueryable<TC> FindOnly(Expression<Func<TC, bool>> predicate);
        TC Find(string id);
        Task<TC> FindAsync(string id);
        void Add(TC entity);
        void Update(TC entity);
        void UpdateRange(List<TC> entities);
        void Delete(string id);
        void Delete(TC entity);
        void Remove(TC entity);
        void InsertUpdateGraph(TC entity);
        void RemoveRange(IEnumerable<TC> lstEntities);
        void AddRange(IEnumerable<TC> lstEntities);
        Task<bool> AnyAsync(Expression<Func<TC, bool>> predicate); // Check if any record exists
    }
}
