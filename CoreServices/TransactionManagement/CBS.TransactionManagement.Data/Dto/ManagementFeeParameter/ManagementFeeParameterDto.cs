using CBS.TransactionManagement.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Dto
{
    public class ManagementFeeParameterDto
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal ManagementFeeFlat { get; set; }
        public decimal ManagementFeeRate { get; set; }
        public string ManagementFeeFrequency { get; set; }
        public string BankId { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public SavingProduct Product { get; set; }
    }
}
