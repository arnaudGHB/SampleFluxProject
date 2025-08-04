using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.FeeP
{
    public class FeePolicyDto
    {
        public string Id { get; set; }
        public string FeeId { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public decimal Value { get; set; }
        public decimal Charge { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string? EventCode { get; set; }
        public bool IsCentralised { get; set; }

        public FeeDto Fee { get; set; }
    }
}
