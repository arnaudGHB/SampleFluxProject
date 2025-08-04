using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class BankTransactionRepository : GenericRepository<BankTransaction, POSContext>, IBankTransactionRepository
    {
        public BankTransactionRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}