using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class AccountingEvent:BaseEntity
    {
        public string Id { get; set; }
        public string EventCode { get; set; }
        public string ChartOfAccountId { get; set; }
        public string OperationEventAttributeId { get; set; }
        public string OperationAccountTypeId { get; set; }
        public string OperationType { get; set; } //Loan_Product,Teller,Saving_Product

    }

}
