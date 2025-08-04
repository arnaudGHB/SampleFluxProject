using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber;
using CBS.CUSTOMER.DATA.Entity.CMoneyP;
using CBS.CUSTOMER.DOMAIN.Context;


namespace CBS.CUSTOMER.REPOSITORY.ChangePhoneNumber
{

    public class PhoneNumberChangeHistoryRepository : GenericRepository<PhoneNumberChangeHistory, POSContext>, IPhoneNumberChangeHistoryRepository
    {
        public PhoneNumberChangeHistoryRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
