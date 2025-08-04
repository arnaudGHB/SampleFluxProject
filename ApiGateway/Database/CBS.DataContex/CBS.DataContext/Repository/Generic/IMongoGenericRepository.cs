using System.Linq.Expressions;

namespace CBS.Gateway.DataContext.Repository.Generic
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
        Task UpdateAsync(object id, TEntity entity);
        Task UpdateManyAsync(IEnumerable<TEntity> entities);
    }
}