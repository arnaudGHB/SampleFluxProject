using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class DailyProvisionTransaction : BaseEntity
    {
        public string Id { get; set; }
        public DateTime date { get; set; }
        public decimal LeftOverAmount { get; set; }
        public decimal ProvisionedAmount { get; set; }
    }
}
