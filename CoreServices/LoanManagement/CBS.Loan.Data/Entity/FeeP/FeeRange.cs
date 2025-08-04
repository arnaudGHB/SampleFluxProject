using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.FeeP
{
    public class FeeRange : BaseEntity
    {
        public string Id { get; set; }
        public string FeeId { get; set; }
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public decimal PercentageValue { get; set; }
        public decimal Charge { get; set; }
        public virtual Fee Fee { get; set; }
    }
}
