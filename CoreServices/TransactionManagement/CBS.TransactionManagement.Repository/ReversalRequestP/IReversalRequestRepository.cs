using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Data.Entity.ReversalRequestP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{
    public interface IReversalRequestRepository : IGenericRepository<ReversalRequest>
    {
    }
}
