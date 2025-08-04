using CBS.Communication.Domain.MongoDBContext.Repository.Generic;

namespace CBS.Communication.Domain.MongoDBContext.Repository.Uow
{
    public interface IMongoUnitOfWork
    {
        void Dispose();
        IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}