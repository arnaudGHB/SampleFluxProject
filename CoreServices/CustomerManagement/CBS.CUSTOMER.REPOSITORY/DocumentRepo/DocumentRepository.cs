using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.Document;
using CBS.CUSTOMER.DOMAIN.Context;

namespace CBS.CUSTOMER.REPOSITORY.DocumentRepo
{

    public class DocumentRepository : GenericRepository<CustomerDocument, POSContext>, IDocumentRepository
    {
        public DocumentRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
