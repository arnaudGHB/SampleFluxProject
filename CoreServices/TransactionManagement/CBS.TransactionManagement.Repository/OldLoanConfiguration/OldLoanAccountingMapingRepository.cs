using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Entity.MobileMoney;
using CBS.TransactionManagement.Data.Entity.OldLoanConfiguration;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.OldLoanConfiguration
{

    public class OldLoanAccountingMapingRepository : GenericRepository<OldLoanAccountingMaping, TransactionContext>, IOldLoanAccountingMapingRepository
    {


        public OldLoanAccountingMapingRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {
        }

    }
}
