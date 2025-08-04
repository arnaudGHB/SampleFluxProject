using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.DailyCollectionManagement.Data.Dto
{
    public class AgentDashBoardDto
    {
        public string Id { get; set; }
        public string KPI { get; set; }
        public string TotalDailyCollection { get; set; }
        public string TotalCashOut { get; set; }
        public string TotalCashIn { get; set; }
        public string TotalLoanRepayment { get; set; }
        public string AgentId { get; set; }
        public string BranchId { get; set; }
    }
}
