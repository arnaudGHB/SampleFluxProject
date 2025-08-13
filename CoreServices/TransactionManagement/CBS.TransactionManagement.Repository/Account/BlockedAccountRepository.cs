using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity;
using CBS.TransactionManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{
   
    public class BlockedAccountRepository : GenericRepository<BlockedAccount, TransactionContext>, IBlockedAccountRepository
    {
        public BlockedAccountRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
