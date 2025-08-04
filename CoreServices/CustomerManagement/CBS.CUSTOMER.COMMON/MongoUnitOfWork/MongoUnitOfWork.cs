using CBS.CUSTOMER.COMMON.MongoGenericRepository;
using CBS.CUSTOMER.DOMAIN.MongoDBContext.DBConnection;

namespace CBS.CUSTOMER.COMMON.MongoUnitOfWork
{
    public class MongoUnitOfWork : IMongoUnitOfWork
    {
        private readonly IMongoDbConnection _mongoDbConnection;
        private readonly Dictionary<Type, object> _repositories;

        public MongoUnitOfWork(IMongoDbConnection mongoDbConnection)
        {
            _mongoDbConnection = mongoDbConnection;
            _repositories = new Dictionary<Type, object>();
        }

        public IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (!_repositories.ContainsKey(entityType))
            {
                var repository = new MongoGenericRepository<TEntity>(_mongoDbConnection);
                _repositories[entityType] = repository;
            }

            return (IMongoGenericRepository<TEntity>)_repositories[entityType];
        }

        public void Dispose()
        {
            // Dispose resources (if necessary)
        }
    }
}
