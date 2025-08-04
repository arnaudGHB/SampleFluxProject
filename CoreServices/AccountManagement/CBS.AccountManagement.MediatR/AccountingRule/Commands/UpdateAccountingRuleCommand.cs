using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Account.
    /// </summary>
 


    public class UpdateAccountingEventRuleCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public bool IsChainEntry { get; set; }
        public string AccountingEventRuleId { get; set; }
        public List<AccountingRule> AccountingRules { get; set; } //Loan Operation xxx
        public List<string> ListOfEligibleBranchId { get; set; }
        public string EventName { get; set; }
        public bool IsDoubleValidationNeeded { get; set; }
        public string LevelOfExecution { get; set; }
        public string EntryType { get; set; }
        public string Description { get; set; }
        public bool IsInterBranchTransaction { get;   set; }

        public class AccountingRule
        {
            public string MFI_ChartOfAccountId { get; set; }
            public string BookingDirection { get; set; }
        }
        public AccountingEventRule ToAccountingEventRule()
        {
            var accountingEventRule = new AccountingEventRule
            {
                // If Id is null or empty, we'll let MongoDB generate it
                Id = this.Id,
                ListOfEligibleBranchId = ListOfEligibleBranchId ?? new List<string>(),
                EventName = EventName,
                IsDoubleValidationNeeded = IsDoubleValidationNeeded,
                LevelOfExecution = LevelOfExecution,
                EntryType = EntryType,
                IsChainEntry = IsChainEntry,
                AccountingEventRuleId = AccountingEventRuleId,
                Description = Description,
                IsInterBranchTransaction = IsInterBranchTransaction,
                // Map AccountingRulex to AccountingEventRule.AccountingRule
                AccountingRules = AccountingRules?.Select(x => new AccountingEventRule.AccountingRule
                {
                    MFI_ChartOfAccountId = x.MFI_ChartOfAccountId,
                    BookingDirection = x.BookingDirection
                }).ToList() ?? new List<AccountingEventRule.AccountingRule>()
            };

            return accountingEventRule;
        }
    }
}