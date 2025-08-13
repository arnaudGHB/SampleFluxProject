using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.FeeP
{
    public class Fee : BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string FeeType { get; set; }//Percentage Or Range Or Flat
        public bool IsAppliesOnHoliday { get; set; }
        public decimal MaximumRateAboveMaximumRange { get; set; }
        public decimal MaximumExtraCharge { get; set; }
        public bool IsMoralPerson { get; set; }
        public string OperationFeeType { get; set; }//MemberShip Or Operation
        public virtual ICollection<FeePolicy> FeePolicies { get; set; }

    }
}
