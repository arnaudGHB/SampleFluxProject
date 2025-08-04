using CBS.AccountManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Repository;

namespace CBS.AccountManagement.Repository
{
    public class AccountingEntryRepository : GenericRepository<Data.AccountingEntry, POSContext>, IAccountingEntryRepository
    {
        public AccountingEntryRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}
