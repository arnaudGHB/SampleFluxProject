using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class EntryTempDataRepository : GenericRepository<EntryTempData, POSContext>, IEntryTempDataRepository
    {
        public EntryTempDataRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}