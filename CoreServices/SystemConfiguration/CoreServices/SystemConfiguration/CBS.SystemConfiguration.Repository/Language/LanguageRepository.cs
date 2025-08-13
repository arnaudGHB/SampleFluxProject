using CBS.SystemConfiguration.Data.Entity;
using CBS.SystemConfiguration.COMMON.GenericRespository;
using CBS.SystemConfiguration.COMMON.UnitOfWork;
using CBS.SystemConfiguration.DOMAIN.Context;

namespace CBS.SystemConfiguration.Repository.Language
{
    public class LanguageRepository : GenericRepository<Language, SystemContext>, ILanguageRepository
    {
        public LanguageRepository(IUnitOfWork<SystemContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
