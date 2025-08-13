using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class ReopenFeeParameter : BaseEntity
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal ReopenFeeFlat { get; set; }
        public decimal ReopenFeeRate { get; set; }
        public string BankId { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public virtual SavingProduct Product { get; set; }
    }

}
