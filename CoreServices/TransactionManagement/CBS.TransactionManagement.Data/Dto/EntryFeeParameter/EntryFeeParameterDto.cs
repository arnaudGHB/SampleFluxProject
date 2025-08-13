using CBS.TransactionManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class EntryFeeParameterDto
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal EntryFeeRate { get; set; }
        public decimal EntryFeeFlat { get; set; }
        public string BankId { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public SavingProduct Product { get; set; }
    }
}
