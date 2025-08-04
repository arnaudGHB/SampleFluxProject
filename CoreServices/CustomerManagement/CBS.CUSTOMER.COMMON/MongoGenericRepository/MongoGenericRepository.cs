
using CBS.CUSTOMER.DOMAIN.MongoDBContext.DBConnection;
using CBS.CUSTOMER.HELPER.Helper.Pagging;
using MongoDB.Driver;

namespace CBS.CUSTOMER.COMMON.MongoGenericRepository
{


    public class MongoGenericRepository<TEntity> : IMongoGenericRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;

        public MongoGenericRepository(IMongoDbConnection mongoDbConnection)
        {
            _collection = mongoDbConnection.Database.GetCollection<TEntity>(typeof(TEntity).Name + "s");
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _collection.Find(FilterDefinition<TEntity>.Empty).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InsertAsync(TEntity entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task InsertManyAsync(IEnumerable<TEntity> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        public async Task UpdateAsync(object id, TEntity entity)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task UpdateManyAsync(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                var id = entity.GetType().GetProperty("CustomerId")?.GetValue(entity, null); // Assuming 'CustomerId' property exists
                var filter = Builders<TEntity>.Filter.Eq("_id", id);
                await _collection.ReplaceOneAsync(filter, entity);
            }
        }

        public async Task DeleteAsync(object id)
        {
            var filter = Builders<TEntity>.Filter.Eq("_id", id);
            await _collection.DeleteOneAsync(filter);
        }

        public async Task DeleteManyAsync(IEnumerable<object> ids)
        {
            var filter = Builders<TEntity>.Filter.In("_id", ids);
            await _collection.DeleteManyAsync(filter);
        }

        /// <summary>
        /// Retrieves documents that match the specified filter.
        /// </summary>
        public async Task<IEnumerable<TEntity>> FindByAsync(FilterDefinition<TEntity> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        /// <summary>
        /// Provides paginated results based on filter, sort, and pagination parameters.
        /// </summary>
        public async Task<PaginatedList<TEntity>> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            FilterDefinition<TEntity> filter = null,
            SortDefinition<TEntity> sort = null,
            CancellationToken cancellationToken = default)
        {
            filter ??= FilterDefinition<TEntity>.Empty;
            var totalCount = (int)await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

            var items = await _collection.Find(filter)
                .Sort(sort)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<TEntity>(items, totalCount, pageNumber, pageSize);
        }

        /// <summary>
        /// Advanced filtering support using a dynamic filter.
        /// </summary>
        /// <param name="filter">The filter definition to apply.</param>
        /// <param name="sort">Optional sorting criteria.</param>
        /// <param name="limit">Optional limit on the number of documents returned.</param>
        /// <param name="cancellationToken">Cancellation token for async operations.</param>
        /// <returns>A list of filtered and optionally sorted documents.</returns>
        public async Task<IEnumerable<TEntity>> GetWithAdvancedFilterAsync(
            FilterDefinition<TEntity> filter,
            SortDefinition<TEntity> sort = null,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            var query = _collection.Find(filter);

            if (sort != null)
            {
                query = query.Sort(sort);
            }

            if (limit.HasValue)
            {
                query = query.Limit(limit.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves documents from the MongoDB collection based on dynamic filters, sorting, and optional limits.
        /// </summary>
        /// <param name="fieldFilters">A dictionary where keys are field names and values are the values to filter by.</param>
        /// <param name="sortField">The field to sort by (optional).</param>
        /// <param name="isDescending">Indicates whether sorting should be in descending order (optional).</param>
        /// <param name="limit">Limits the number of returned documents (optional).</param>
        /// <param name="cancellationToken">Token to cancel the operation (optional).</param>
        /// <returns>A list of documents matching the provided filters.</returns>
        /// <remarks>
        /// This method dynamically builds MongoDB queries using the provided dictionary of field-value pairs.
        /// It supports optional sorting by a specified field and limiting the number of results returned.
        /// </remarks>
        public async Task<IEnumerable<TEntity>> GetWithGenericFilterAsync(
            Dictionary<string, object> fieldFilters,
            string sortField = null,
            bool isDescending = false,
            int? limit = null,
            CancellationToken cancellationToken = default)
        {
            // Build the filter dynamically
            var filterBuilder = Builders<TEntity>.Filter;
            var filters = new List<FilterDefinition<TEntity>>();

            foreach (var fieldFilter in fieldFilters)
            {
                filters.Add(filterBuilder.Eq(fieldFilter.Key, fieldFilter.Value));
            }

            var combinedFilter = filters.Any() ? filterBuilder.And(filters) : FilterDefinition<TEntity>.Empty;

            // Apply sorting if specified
            SortDefinition<TEntity> sort = null;
            if (!string.IsNullOrEmpty(sortField))
            {
                sort = isDescending
                    ? Builders<TEntity>.Sort.Descending(sortField)
                    : Builders<TEntity>.Sort.Ascending(sortField);
            }

            // Query the collection
            var query = _collection.Find(combinedFilter);

            if (sort != null)
            {
                query = query.Sort(sort);
            }

            if (limit.HasValue)
            {
                query = query.Limit(limit.Value);
            }

            return await query.ToListAsync(cancellationToken);
        }

    }
}
