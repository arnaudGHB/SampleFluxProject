using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class InitializeTellerAccountCommand : IRequest<ServiceResponse<AccountDto>>
    {
        public string Id { get; set; }
        public string Status { get; set; } = AccountStatus.Open.ToString();
        public decimal Amount { get; set; }

        public InitializeTellerAccountCommand(string id, decimal amount)
        {
            Id = id;
            Amount = amount;
        }
    }

}
