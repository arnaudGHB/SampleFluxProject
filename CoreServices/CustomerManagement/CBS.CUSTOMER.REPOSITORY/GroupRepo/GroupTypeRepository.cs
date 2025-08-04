using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.Groups;
using CBS.CUSTOMER.DOMAIN.Context;

namespace CBS.CUSTOMER.REPOSITORY
{

    public class GroupTypeRepository : GenericRepository<GroupType, POSContext>, IGroupTypeRepository
    {
        public GroupTypeRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
