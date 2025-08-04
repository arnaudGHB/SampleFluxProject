using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a Account.
    /// </summary>
    public class UpdateAccountCommand : IRequest<ServiceResponse<AccountDto>>
    {
        public string Id { get; set; }
        // Account Number
        public string AccountNumber { get; set; }
        // Account Holder
        public string AccountHolder { get; set; }
        // Account Description
        public string AccountOwnerId { get; set; }
        public string AccountTypeId { get; set; }
        // Currency Code

        public AccountStatus Status { get; set; }
    }
}