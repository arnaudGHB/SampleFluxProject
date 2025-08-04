using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class TransactionDataRepository : GenericRepository<TransactionData, POSContext>, ITransactionDataRepository
    {
        public TransactionDataRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}