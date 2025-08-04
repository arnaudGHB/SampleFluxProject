using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Enums
{
    public enum ScheduleEnum
    {
        maturity_quarterly_actual_360, 
        maturity_quarterly_actual_360_domestic,
        fixed_principal_quarterly, 
        fixed_principal_bi_annually, 
        fixed_principal_annually,
        fixed_principal_biweekly, 
        fixed_principal_two_monthly, 
        fixed_principal_monthly, 
        flat_monthly,
        flat_quarterly,
        flat_biweekly,
        flat_annually,
        flat_two_monthly, 
        flat_bi_annually, 
        differential_bi_annually, 
        annuity_monthly,
        annuity_quarterly, 
        annuity_bi_annually,
        annuity_two_monthly,
        annuity_biweekly,
        annuity_annually,
        annuity_monthly_fact, 
        annuity_quarterly_fact,
        annuity_bi_annually_fact, 
        differential_quarterly

    }
}
