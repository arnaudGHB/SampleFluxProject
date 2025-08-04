using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data.Enum
{
    public static class  AccountingEntryQueryOptions
    {

        /// <summary>
        /// Get entries at Branch Level
        /// </summary>
        public static string EntriesOfBranch { get; set; }= "BRANCH_ENTRIES"; //EntriesOfBranch,

        /// <summary>
        /// Get entries at Bank Level
        /// </summary>
        public static string EntriesOfBank { get; set; } = "BANK_ENTRIES";
    
        /// <summary>
        /// Get deleted entries at Branch Level
        /// </summary>
        public static string DeletedEntriesOfBranch { get; set; } = "DELETED_BRANCH_ENTRIES";
     
   
         /// <summary>
        /// Get deleted entries at Branch Level
        /// </summary>
         public static string DeletedEntriesOfBank { get; set; } = "DELETED_BANK_ENTRIES";
    }
}
