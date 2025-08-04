using System.Linq.Expressions;

namespace CBS.NLoan.Common.MongoGenericRepository
{
    using CBS.NLoan.Helper.Helper.Pagging;

    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IMongoGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetByIdAsync(object id);

        Task InsertAsync(TEntity entity);

        Task InsertManyAsync(IEnumerable<TEntity> entities);

        Task UpdateAsync(object id, TEntity entity);

        Task UpdateManyAsync(IEnumerable<TEntity> entities);

        Task DeleteAsync(object id);

        Task DeleteManyAsync(IEnumerable<object> ids);

        /// <summary>
        /// Retrieves documents that match the specified filter.
        /// </summary>
        Task<IEnumerable<TEntity>> FindByAsync(FilterDefinition<TEntity> filter);

        /// <summary>
        /// Provides paginated results based on filter, sort, and pagination parameters.
        /// </summary>
        Task<PaginatedList<TEntity>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            FilterDefinition<TEntity> filter = null,
            SortDefinition<TEntity> sort = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Advanced filtering support using a dynamic filter.
        /// </summary>
        Task<IEnumerable<TEntity>> GetWithAdvancedFilterAsync(
            FilterDefinition<TEntity> filter,
            SortDefinition<TEntity> sort = null,
            int? limit = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves documents from the MongoDB collection based on dynamic filters, sorting, and optional limits.
        /// </summary>
        Task<IEnumerable<TEntity>> GetWithGenericFilterAsync(
            Dictionary<string, object> fieldFilters,
            string sortField = null,
            bool isDescending = false,
            int? limit = null,
            CancellationToken cancellationToken = default);
    }

}