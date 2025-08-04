using CBS.NLoan.Data.Entity.LoanApplicationP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.CollateraP
{
    public class Collateral : BaseEntity
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<LoanProductCollateral> LoanProductCollaterals { get; set; }
    }
}
