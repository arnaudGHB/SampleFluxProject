using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.CMoneyP;
using CBS.CUSTOMER.DOMAIN.Context;


namespace CBS.CUSTOMER.REPOSITORY.CMoney.SubcriptionDetail
{

    public class CMoneySubscriptionDetailRepository : GenericRepository<CMoneySubscriptionDetail, POSContext>, ICMoneySubscriptionDetailRepository
    {
        public CMoneySubscriptionDetailRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
