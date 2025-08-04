using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Entity.MobileMoney;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.MobileMoney
{
    public interface IMobileMoneyCashTopupRepository : IGenericRepository<MobileMoneyCashTopup>
    {
        Task<List<MobileMoneyCashTopupDto>> GetMobileMoneyCash(string QuerParamter, bool ByBranch, string BranchId);
    }
}
