using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new AccountingRuleEntryDto.
    /// </summary>
    public class AddAccountingRuleEntryCommand : IRequest<ServiceResponse<AccountingRuleEntryDto>>
    {
        public string? AccountingRuleEntryName { get; set; }
        public string BookingDirection { get; set; }
        public string? OperationEventAttributeId { get; set; }
        public string DeterminationAccountId { get; set; }
        public string BalancingAccountId { get; set; }
        public string? BankId { get; set; }

        public string Description { get; set; } = string.Empty;

    }
 
    

}