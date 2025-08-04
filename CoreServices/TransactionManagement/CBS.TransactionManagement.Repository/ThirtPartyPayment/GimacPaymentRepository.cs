using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.ThirtPartyPayment;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Repository.ThirtPartyPayment
{

    public class GimacPaymentRepository : GenericRepository<GimacPayment, TransactionContext>, IGimacPaymentRepository
    {
        public GimacPaymentRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
