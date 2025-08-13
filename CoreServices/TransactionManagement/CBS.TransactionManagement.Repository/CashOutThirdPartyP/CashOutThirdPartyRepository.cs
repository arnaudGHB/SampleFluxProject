using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.CashOutThirdPartyP;
using CBS.TransactionManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.CashOutThirdPartyP
{
   
    public class CashOutThirdPartyRepository : GenericRepository<CashOutThirdParty, TransactionContext>, ICashOutThirdPartyRepository
    {
        public CashOutThirdPartyRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
