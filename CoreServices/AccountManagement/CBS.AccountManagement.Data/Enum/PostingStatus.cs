using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data.Enum
{
    //    INCOME
    //
    //
    //
    public enum TellerSources
    {
        Virtual_Teller_MTN,
        Virtual_Teller_Orange,
        Virtual_Teller_GAV,
        Members_Account,
        Members_Activation,
        Physical_Teller,
        Virtual_Teller_Momo_cash_Collection,
        DailyCollector
    }
    public enum TypeOfEntry
    {
        TRUST_SYSTEM,
        USER
    }
    // 
    public static class PostingStatus
    {
        public static string Posted { get; set; } = "Posted";
        public static string Drafted { get; set; } = "Drafted";
        public static string Approved { get; set; } = "Approved";
        public static string Pending { get; set; } = "Pending";
        public static string Rejected { get; set; } = "Rejected";
        public static string Reversed { get; set; } = "Reversed";
    }
    public static class FinancialStatement
    {
        public static string Income { get; set; } = "INCOME";
        public static string Liability { get; set; } = "Liability";
        public static string Expense { get; set; } = "Expense";
        public static string Asset { get; set; } = "ASSETS";
        
    }

    public static class BalanceSheetCartegory
    {
        public static string GROSS { get; set; } = "GROSS";
        public static string PROVISION { get; set; } = "PROVISION";
 
    }
}
