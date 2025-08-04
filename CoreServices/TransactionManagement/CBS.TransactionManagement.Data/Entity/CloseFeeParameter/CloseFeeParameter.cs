using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class CloseFeeParameter:BaseEntity
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal CloseFeeFlat { get; set; }
        public decimal CloseFeeRate { get; set; }
        public string OperationEventAttributeName { get; set; }
        public bool? IsDebit { get; set; }
        public string? ChartOfAccountId { get; set; }
        public string BankId { get; set; }

        public virtual SavingProduct Product { get; set; }
    }
}
