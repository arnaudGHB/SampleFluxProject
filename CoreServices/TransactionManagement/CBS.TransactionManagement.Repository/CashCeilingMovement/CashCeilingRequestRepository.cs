using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.CashCeilingMovement;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Repository.CashCeilingMovement
{

    public class CashCeilingRequestRepository : GenericRepository<CashCeilingRequest, TransactionContext>, ICashCeilingRequestRepository
    {
        public CashCeilingRequestRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
