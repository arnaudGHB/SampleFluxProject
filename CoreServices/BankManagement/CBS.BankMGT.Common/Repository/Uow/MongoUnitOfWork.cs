using CBS.BankMGT.Common.DBConnection;
using CBS.BankMGT.Common.Repository.Generic;

namespace CBS.BankMGT.Common.Repository.Uow
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
            try
            {
                var entityType = typeof(TEntity);

                if (!_repositories.ContainsKey(entityType))
                {
                    var repository = new MongoGenericRepository<TEntity>(_mongoDbConnection);
                    _repositories[entityType] = repository;
                }

                return (IMongoGenericRepository<TEntity>)_repositories[entityType];
            }
            catch (Exception ex)
            {

                var messsage =$"Error initializing repository for {typeof(TEntity).Name}: {ex}";
                throw;
            }
        }

        public void Dispose()
        {
            // Dispose resources (if necessary)
        }
    }
}
