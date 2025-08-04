using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity.Groups;
using CBS.CUSTOMER.DOMAIN.Context;

namespace CBS.CUSTOMER.REPOSITORY
{

    public class GroupDocumentRepository : GenericRepository<GroupDocument, POSContext>, IGroupDocumentRepository
    {
        public GroupDocumentRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
