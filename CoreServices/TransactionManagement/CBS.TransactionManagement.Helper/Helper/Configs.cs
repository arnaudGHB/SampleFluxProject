using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper
{
    public enum SystemConfigs
    {
        tax,
        StartOfDayAlert,
        EndOFDayAlert,
        IsDayOpen,
        IsYearOpen,
        EndOfYearDate,
        StartOfYearDate,
    }

    public enum DefaultProducts
    {
        DAILYOPERATIONS
    }
}
