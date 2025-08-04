using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data
{
    public class AccountingEventRule:BaseEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// This indicated if double validation is required.
        /// </summary>

        public List<AccountingRule> AccountingRules { get; set; } //Loan Operation xxx
        public string Description { get; set; }
        public List<string> ListOfEligibleBranchId { get; set; }
        /// <summary>
        /// List of the branches that execution shall take place in their accounting system
        /// </summary>
        public string EventName { get; set; }
        public bool IsDoubleValidationNeeded { get; set; }

        public bool IsChainEntry { get; set; }

        public string AccountingEventRuleId { get; set; }
        /// <summary>
        /// The LevelOfExecution indicate the scope of execution of an accounting entry 
        /// </summary>
        public string LevelOfExecution { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EntryType { get; set; }
        public bool IsInterBranchTransaction { get; set; }
        public class AccountingRule
        {
            public string MFI_ChartOfAccountId { get; set; }
            public string BookingDirection { get; set; }
        }
    }

   
}
