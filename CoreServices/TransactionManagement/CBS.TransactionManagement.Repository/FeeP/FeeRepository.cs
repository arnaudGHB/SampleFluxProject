using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Repository.FeeP
{

    public class FeeRepository : GenericRepository<Fee, TransactionContext>, IFeeRepository
    {
        public FeeRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
