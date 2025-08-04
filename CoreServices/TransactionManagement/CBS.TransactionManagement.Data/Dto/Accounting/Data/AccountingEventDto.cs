using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class AccountingEventDto
    {
        public string Id { get; set; }
        public string EventCode { get; set; }
        public string ChartOfAccountId { get; set; }
        public string OperationEventAttributeId { get; set; }
        public string ProductID { get; set; }
    }
}
