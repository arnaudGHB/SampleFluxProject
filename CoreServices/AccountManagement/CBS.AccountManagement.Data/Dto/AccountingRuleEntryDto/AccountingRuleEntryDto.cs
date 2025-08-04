using CBS.AccountManagement.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class AccountingRuleEntryDto 
    {
        public string Id { get; set; }
        public bool IsPrimaryEntry { get; set; }
        public string EventCode { get; set; }
        public string AccountingRuleEntryName { get; set; }
        public string BookingDirection { get; set; }
        public string? OperationEventAttributeId { get; set; }
        public string? ProductAccountingBookId { get; set; }
        public string? DeterminationAccountId { get; set; }
        public bool? HasManagementAccountId { get; set; }
        public string? BalancingAccountId { get; set; }
        public string BankId { get; set; }
        public string? OperationEventId { get; set; }
        public string Description { get; set; } = string.Empty;

    }

    public class RuleEntryDto
    {
        public string? AccountingRuleEntryName { get; set; }
        public string EventCode { get; set; } = string.Empty;

    }


    public class RuleEntries
    {
        public string? AccountingRuleEntryName { get; set; }
        public string Id { get; set; } = string.Empty;

    }

    public class AccountMap
    {
        public string? AccountName { get; set; }
        public string AccountNumber { get; set; }

    }

    public class AccountingRuleEntryDtos
    {
        public string Id { get; set; }
        public string OperationEventId { get; set; }
        public string OperationEventAttributeId { get; set; }
        public string DrAccountId { get; set; }
        public string DrDescription { get; set; }
        public string CrAccountId { get; set; }
        public string CrDescription { get; set; }
        public string BookingDirection { get; set; }
        public string EventCode { get; set; }
        public bool? HasManagementAccountId { get; set; }
        public string? BalancingAccountId { get; set; }
        public string? DeterminationManagementAccountId { get; set; }
        public bool IsPrimaryEntry { get; set; }
    }

}
