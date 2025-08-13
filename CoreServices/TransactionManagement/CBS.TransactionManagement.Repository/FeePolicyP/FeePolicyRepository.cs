using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Domain;

namespace CBS.NLoan.Repository.FeePolicyP
{

    public class FeePolicyRepository : GenericRepository<FeePolicy, TransactionContext>, IFeePolicyRepository
    {
        public FeePolicyRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
