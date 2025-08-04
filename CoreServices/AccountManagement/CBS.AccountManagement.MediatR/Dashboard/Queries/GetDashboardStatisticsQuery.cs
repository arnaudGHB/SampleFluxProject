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
    public class GetDashboardStatisticsQuery : IRequest<ServiceResponse<List<DashboardStatisticsDto>>>
    {
        public string BranchId { get; set; }
        public string QueryParameter { get; set; }
    }

 
 
}
