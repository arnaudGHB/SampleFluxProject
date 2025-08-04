using CBS.NLoan.Common.MongoGenericRepository;

namespace CBS.NLoan.Common.MongoUnitOfWork
{
    public interface IMongoUnitOfWork
    {
        void Dispose();
        IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}