using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using CBS.TransactionManagement.Data.Entity.VaultOperationP;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Repository.VaultOperationP
{

    public class VaultOperationRepository : GenericRepository<VaultOperation, TransactionContext>, IVaultOperationRepository
    {
        public VaultOperationRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
