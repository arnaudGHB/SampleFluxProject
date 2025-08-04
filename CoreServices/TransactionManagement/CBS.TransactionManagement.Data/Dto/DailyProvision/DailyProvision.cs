using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class DailyProvisionDto
    {
        public string Id { get; set; }
        public DateTime date { get; set; }
        public decimal LeftOverAmount { get; set; }
        public decimal ProvisionedAmount { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
