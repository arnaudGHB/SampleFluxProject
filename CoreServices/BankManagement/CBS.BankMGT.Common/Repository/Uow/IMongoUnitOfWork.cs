using CBS.BankMGT.Common.Repository.Generic;

namespace CBS.BankMGT.Common.Repository.Uow
{
    public interface IMongoUnitOfWork
    {
        void Dispose();
        IMongoGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}