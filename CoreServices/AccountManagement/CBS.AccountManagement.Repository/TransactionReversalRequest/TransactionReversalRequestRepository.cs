using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Repository 
{
    
    public class TransactionReversalRequestRepository : GenericRepository<TransactionReversalRequest, POSContext>, ITransactionReversalRequestRepository
    {
        public TransactionReversalRequestRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
