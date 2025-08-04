using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DOMAIN.Context;


namespace CBS.CUSTOMER.REPOSITORY.OrganisationRepo
{

    public class OrganizationRepository : GenericRepository<Organization, POSContext>, IOrganizationRepository
    {
        public OrganizationRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
