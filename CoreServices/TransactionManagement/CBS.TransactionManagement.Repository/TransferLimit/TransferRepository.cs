using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.TransferLimits;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Repository
{

    public class TransferRepository : GenericRepository<Transfer, TransactionContext>, ITransferRepository
    {
        public TransferRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
