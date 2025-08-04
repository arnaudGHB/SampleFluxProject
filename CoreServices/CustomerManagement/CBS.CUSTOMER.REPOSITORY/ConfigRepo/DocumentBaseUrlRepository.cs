using CBS.CUSTOMER.COMMON.GenericRespository;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Entity.Config;
using CBS.CUSTOMER.DOMAIN.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.REPOSITORY
{

    public class DocumentBaseUrlRepository : GenericRepository<DocumentBaseUrl, POSContext>, IDocumentBaseUrlRepository
    {
        public DocumentBaseUrlRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
