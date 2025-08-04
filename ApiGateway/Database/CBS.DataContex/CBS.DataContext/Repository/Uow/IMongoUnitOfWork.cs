using CBS.Gateway.DataContext.Repository.Generic;

namespace CBS.Gateway.DataContext.Repository.Uow
{
    public interface IMongoUnitOfWork
    {
        void Dispose();
        IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}