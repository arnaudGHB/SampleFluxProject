using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Repository.VaultAuthorisedPersonP
{

    public class VaultAuthorisedPersonRepository : GenericRepository<VaultAuthorisedPerson, TransactionContext>, IVaultAuthorisedPersonRepository
    {
        public VaultAuthorisedPersonRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
