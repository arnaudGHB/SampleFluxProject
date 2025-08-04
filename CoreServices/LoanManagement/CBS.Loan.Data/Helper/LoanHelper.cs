using CBS.NLoan.Data.Entity.FeeP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Helper
{
    public static class LoanHelper
    {
        public static bool HasUnpaidFees(List<LoanApplicationFee> fees,string period)
        {
            var Payments = fees.Where(x => x.Period == period).ToList();
            return Payments.Any(x => !x.IsPaid);

        }
       
    }
}
