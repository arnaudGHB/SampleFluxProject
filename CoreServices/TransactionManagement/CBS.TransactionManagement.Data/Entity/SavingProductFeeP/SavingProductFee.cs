using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Data.Entity.FeeP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.SavingProductFeeP
{
    public class SavingProductFee : BaseEntity
    {
        public string Id { get; set; }
        public string FeeId { get; set; }
        public string SavingProductId { get; set; }
        public string FeeType { get; set; }//Withdrawal, Tranfer Or Cash-in
        public string FeePolicyType { get; set; }

        public virtual Fee Fee { get; set; }
        public virtual SavingProduct SavingProduct { get; set; }
    }
}
