using MongoDB.Driver;
using System.Linq.Expressions;

namespace CBS.BankMGT.Common.Repository.Generic
{
    public interface IMongoGenericRepository<TEntity> where TEntity : class
    {
        Task DeleteAsync(object id);
        Task DeleteManyAsync(IEnumerable<object> ids);
        IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(object id);
        Task InsertAsync(TEntity entity);
        Task InsertManyAsync(IEnumerable<TEntity> entities);
        Task<long> CountAsync(FilterDefinition<TEntity> filter = null);
        Task UpdateAsync(object id, TEntity entity);
        Task UpdateManyAsync(IEnumerable<TEntity> entities);
        Task<List<TEntity>> GetPagedAsync(int skip, int take);
        Task<long> CountAsync();
        Task<List<TEntity>> GetPagedFilteredAndSortedAsync(FilterDefinition<TEntity> filter, SortDefinition<TEntity> sort, int skip, int take);
        IMongoCollection<TEntity> Collection { get; }
    }
}