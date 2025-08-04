using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data
{
    public class LoanProductCollateral : BaseEntity
    {
        public string Id { get; set; }
        public string CollateralId { get; set; }
        public string LoanProductId { get; set; }
        public string LoanProductCollateralTag { get; set; }
        public decimal MinimumValueRate { get; set; }
        public decimal MaximumValueRate { get; set; }
        public virtual Collateral Collateral { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }
        public virtual ICollection<LoanApplicationCollateral> LoanApplicationCollaterals { get; set; }
        
    }
}
