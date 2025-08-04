using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using CBS.TransactionManagement.Data.Entity.VaultOperationP;

namespace CBS.TransactionManagement.Repository.VaultOperationP
{
    public interface IVaultOperationRepository : IGenericRepository<VaultOperation>
    {
    }
}
