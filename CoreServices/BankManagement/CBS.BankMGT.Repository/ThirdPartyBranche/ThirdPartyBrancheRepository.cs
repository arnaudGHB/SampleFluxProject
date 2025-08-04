using CBS.BankMGT.Common.GenericRespository;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Entity;
using CBS.BankMGT.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Repository
{
   
    public class ThirdPartyBrancheRepository : GenericRepository<ThirdPartyBranche, POSContext>, IThirdPartyBrancheRepository
    {
        public ThirdPartyBrancheRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
