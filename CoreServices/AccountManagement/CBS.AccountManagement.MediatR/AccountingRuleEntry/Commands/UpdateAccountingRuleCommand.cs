using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Account.
    /// </summary>
    public class UpdateAccountingRuleEntryCommand : IRequest<ServiceResponse<AccountingRuleEntryDto>>
    {
        public string Id { get; set; }
        public string? AccountingRuleEntryName { get; set; }
        public string BookingDirection { get; set; }
        public string? OperationEventAttributeId { get; set; }
        public string? OperationEventId { get; set; }
        public string? DeterminationAccountId { get; set; }
        public string? BalancingAccountId { get; set; }
        public bool? HasManagementAccountId { get; set; }
        public string? Description { get; set; }
        public string? DeterminationManagementAccountId { get; set; }
        public string? BankId { get; set; }
    }
}