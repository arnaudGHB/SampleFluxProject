using CBS.CUSTOMER.COMMON.MongoGenericRepository;

namespace CBS.CUSTOMER.COMMON.MongoUnitOfWork
{
    public interface IMongoUnitOfWork
    {
        void Dispose();
        IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}