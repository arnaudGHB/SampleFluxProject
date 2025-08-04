using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DOMAIN.Context;

namespace CBS.CUSTOMER.REPOSITORY
{

    public class OrganizationCustomerRepository : GenericRepository<OrganizationCustomer, POSContext>, IOrganizationCustomerRepository
    {
        public OrganizationCustomerRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
