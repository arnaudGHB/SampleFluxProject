using CBS.DailyCollectionManagement.Common.DBConnection;



namespace CBS.DailyCollectionManagement.Common
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

    public class MongoUnitOfWork2 : IMongoUnitOfWork
    {
        private readonly IMongoDb2Connection _mongoDbConnection;
        private readonly Dictionary<Type, object> _repositories;

        public MongoUnitOfWork2(IMongoDb2Connection mongoDbConnection)
        {
            _mongoDbConnection = mongoDbConnection;
            _repositories = new Dictionary<Type, object>();
        }

        public IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (!_repositories.ContainsKey(entityType))
            {
                var repository = new Mongo2GenericRepository<TEntity>(_mongoDbConnection);
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
