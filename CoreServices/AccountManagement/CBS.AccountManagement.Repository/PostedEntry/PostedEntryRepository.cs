using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
 
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class PostedEntryRepository : GenericRepository<PostedEntry, POSContext>, IPostedEntryRepository
    {
        public PostedEntryRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}