using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Entity.LoanApplicationP
{
    public class LoanProductMaturityPeriodExtension:BaseEntity
    {
        public string Id { get; set; }
        public bool ExternLoanAfterMaturityPeriod { get; set; }
        public string MaturityPeriodLoanInterestType { get; set; }
        public string CalculateInterestOn { get; set; }
        public double InterestRate { get; set; }
        public string LoanProductId { get; set; }
        public int RecurringPeriod { get; set; }
        public string RecurringPeriodType { get; set; }
        public bool IncludePenalTyFee { get; set; }
        public bool KeepLoanStatusAsPAssedMaturityEvenAfterLoanIsExterneded { get; set; }
        public virtual LoanProduct LoanProduct { get; set; }
    }
}
