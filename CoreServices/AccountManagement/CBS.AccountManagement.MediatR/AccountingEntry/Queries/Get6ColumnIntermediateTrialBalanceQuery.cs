using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Queries
{
    public class Get6ColumnIntermediateTrialBalanceQuery : SystemQuery,IRequest<ServiceResponse<TrialBalance6ColumnDto>>
    {
        
    }
}
