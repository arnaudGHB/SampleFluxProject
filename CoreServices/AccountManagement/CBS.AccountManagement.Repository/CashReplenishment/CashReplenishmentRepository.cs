using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class CashReplenishmentRepository : GenericRepository<CashReplenishment, POSContext>, ICashReplenishmentRepository
    {
        public CashReplenishmentRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}