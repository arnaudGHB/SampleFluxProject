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
    public class Get4ColumnIntermediateTrialBalanceQuery : IRequest<ServiceResponse<TrialBalance4ColumnDto>>
    {
        public DateTime ToDate { get; set; }
        public DateTime FromDate { get; set; }
    
        public string QueryLevel { get;   set; }
        public List<string> BranchId { get;   set; }
    }
}
