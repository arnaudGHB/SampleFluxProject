using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DOMAIN.Context;


namespace CBS.CUSTOMER.REPOSITORY
{

    public class UploadedCustomerWithErrorRepository : GenericRepository<UploadedCustomerWithError, POSContext>, IUploadedCustomerWithErrorRepository
    {
        public UploadedCustomerWithErrorRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
