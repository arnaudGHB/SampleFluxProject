using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class AddRuleEntryCommand : IRequest<bool>
    {
        public List<RuleEntry> ruleEntries { get; set; } = new List<RuleEntry>();
    }

    public class RuleEntry
    {
        public string? AccountingRuleEntryName { get; set; }
        public string? OperationEventAttributeId { get; set; }
        public string DeterminationAccountId { get; set; }
        public string BalancingAccountId { get; set; }
        public string? AccountTypeId { get; set; }
        public string? BankId { get; set; } = null;
        public bool IsPrimaryEntry { get; internal set; } // true if debit account
    }
}
