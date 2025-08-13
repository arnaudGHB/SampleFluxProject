using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CBS.UserSkillManagement.Common
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetPaged(int pageNumber, int pageSize);
        Task SaveChangesAsync();
    }
}
