using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DOMAIN.Context;

namespace CBS.CUSTOMER.REPOSITORY
{

    public class GroupCustomerRepository : GenericRepository<GroupCustomer, POSContext>, IGroupCustomerRepository
    {
        public GroupCustomerRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
