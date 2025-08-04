using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class DeleteAccountCommand : IRequest<ServiceResponse<bool>>
    {
        public string AccountId { get; set; }

        public DeleteAccountCommand(string accountId)
        {
            AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
        }
    }


}
