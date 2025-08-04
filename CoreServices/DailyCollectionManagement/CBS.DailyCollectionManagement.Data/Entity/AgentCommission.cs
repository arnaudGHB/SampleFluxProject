using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.DailyCollectionManagement.Data.Entity
{
    public class AgentCommission
    {
        public string Id { get; set; }
        public string Balance { get; set; }
        public string CashIn { get; set; }
        public string CashOut { get; set; }
        public string AgentId { get; set; }
        public string BranchId { get; set; }
    }
}
