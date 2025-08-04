using CBS.NLoan.Data.Entity.LoanApplicationP;
using System.ComponentModel.DataAnnotations;

namespace CBS.NLoan.Data.Entity.AccountingRuleP
{
    public class AccountingRule : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string AccountingRuleId { get; set; }
        public string LoanProductId { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }
    }
}
