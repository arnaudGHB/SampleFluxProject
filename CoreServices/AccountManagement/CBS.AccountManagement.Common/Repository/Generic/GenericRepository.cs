using CBS.AccountManagement.Common.DBConnection;
using CBS.AccountManagement.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace CBS.AccountManagement.Common
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
                var id = entity.GetType().GetProperty("Id")?.GetValue(entity, null); // Assuming 'Id' property exists
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

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _collection.AsQueryable().Where(predicate);
        }
    }

    public class Mongo2GenericRepository<TEntity> : IMongoGenericRepository<TEntity> where TEntity : class
    {
        private readonly IMongoCollection<TEntity> _collection;

        public Mongo2GenericRepository(IMongoDb2Connection mongoDbConnection)
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
                var id = entity.GetType().GetProperty("Id")?.GetValue(entity, null); // Assuming 'Id' property exists
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

        public IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return _collection.AsQueryable().Where(predicate);
        }
    }
}
