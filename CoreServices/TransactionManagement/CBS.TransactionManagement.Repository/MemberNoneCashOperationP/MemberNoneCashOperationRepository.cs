using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Data.Entity.MemberNoneCashOperationP;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Repository.MemberNoneCashOperationP
{

    public class MemberNoneCashOperationRepository : GenericRepository<MemberNoneCashOperation, TransactionContext>, IMemberNoneCashOperationRepository
    {
        public MemberNoneCashOperationRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
