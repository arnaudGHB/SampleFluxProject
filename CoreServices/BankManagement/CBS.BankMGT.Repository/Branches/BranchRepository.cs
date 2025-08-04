using CBS.BankMGT.Common.GenericRespository;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Repository
{
   
    public class BranchRepository : GenericRepository<Branch, POSContext>, IBranchRepository
    {
        public BranchRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
