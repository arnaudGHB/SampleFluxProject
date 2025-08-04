using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class ProductAccountingBookRepository : GenericRepository<ProductAccountingBook, POSContext>, IProductAccountingBookRepository
    {
        public ProductAccountingBookRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}