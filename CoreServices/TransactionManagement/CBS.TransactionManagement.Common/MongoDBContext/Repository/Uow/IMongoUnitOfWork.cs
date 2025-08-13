using CBS.TransactionManagement.Common.Repository.Generic;

namespace CBS.TransactionManagement.Common.Repository.Uow
{
    public interface IMongoUnitOfWork
    {
        void Dispose();
        IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}