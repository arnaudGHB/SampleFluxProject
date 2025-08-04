using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data.Enum
{
    public static class AccountingEvent
    {
        public static string NORMAL_OOD { get; set; } = "OOD";
        public static string NEGATIVE_OOD { get; set; } = "NEGATIVE_OOD";
        public static string POSITIVE_OOD { get; set; } = "POSITIVE_OOD";
        public static string NORMAL_COD { get; set; } = "COD";
        public static string NEGATIVE_COD { get; set; } = "NEGATIVE_COD";
        public static string POSITIVE_COD { get; set; } = "POSITIVE_COD";
    }
}
