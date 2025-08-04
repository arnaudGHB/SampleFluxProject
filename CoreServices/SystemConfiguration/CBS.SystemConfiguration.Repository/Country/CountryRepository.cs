using CBS.SystemConfiguration.Common;

using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;

namespace CBS.SystemConfiguration.Repository
{
    public class CountryRepository : GenericRepository<Country, SystemContext>, ICountryRepository
    {
        public CountryRepository(IUnitOfWork<SystemContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}