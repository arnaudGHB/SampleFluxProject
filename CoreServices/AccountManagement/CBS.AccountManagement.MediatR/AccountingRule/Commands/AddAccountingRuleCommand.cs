using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AccountingRuleDto.
    /// </summary>
    public class AddAccountingRuleCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public List<AccountingRulex> AccountingRules { get; set; } //Loan Operation xxx
        public List<string> ListOfEligibleBranchId { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public bool IsDoubleValidationNeeded { get; set; }
        public string LevelOfExecution { get; set; }
        public string EntryType { get;   set; }
        public bool IsChainEntry { get; set; }
        public bool IsInterBranchTransaction { get; set; }
        public string? AccountingEventRuleId { get; set; }
        public AccountingEventRule ToAccountingEventRule()
    {
        var accountingEventRule = new AccountingEventRule
        {
            // If Id is null or empty, we'll let MongoDB generate it
            Id = string.IsNullOrEmpty(Id) ? null : BaseUtilities.GenerateInsuranceUniqueNumber(15,"AER"),
            ListOfEligibleBranchId = ListOfEligibleBranchId ?? new List<string>(),
            EventName = EventName,
            IsDoubleValidationNeeded = IsDoubleValidationNeeded,
            Description = Description,
            LevelOfExecution = LevelOfExecution,
            EntryType = EntryType,
            IsInterBranchTransaction = IsInterBranchTransaction,
            AccountingEventRuleId = AccountingEventRuleId,
            IsChainEntry = IsChainEntry,
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

    public class  AccountingRulex
    {
        public string? RuleName { get; set; } //Loan Operation xxx
        public string MFI_ChartOfAccountId { get; set; }
        public string BookingDirection { get; set; }
 
    }
}