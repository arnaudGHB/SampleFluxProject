using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.ReopenFeeParameterP
{
   
    public class ReopenFeeParameterRepository : GenericRepository<ReopenFeeParameter, TransactionContext>, IReopenFeeParameterRepository
    {
        public ReopenFeeParameterRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
