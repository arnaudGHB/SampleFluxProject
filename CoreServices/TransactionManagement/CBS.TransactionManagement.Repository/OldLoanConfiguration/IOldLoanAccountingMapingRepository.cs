using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Entity.MobileMoney;
using CBS.TransactionManagement.Data.Entity.OldLoanConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.OldLoanConfiguration
{
    public interface IOldLoanAccountingMapingRepository : IGenericRepository<OldLoanAccountingMaping>
    {
    }
}
