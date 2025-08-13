using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.SavingProductFeeP;
using CBS.TransactionManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{

    public class SavingProductFeeRepository : GenericRepository<SavingProductFee, TransactionContext>, ISavingProductFeeRepository
    {
        public SavingProductFeeRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
