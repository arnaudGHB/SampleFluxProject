using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class OperationEventAttributesRepository : GenericRepository<OperationEventAttributes, POSContext>, IOperationEventAttributeRepository
    {
        public OperationEventAttributesRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}