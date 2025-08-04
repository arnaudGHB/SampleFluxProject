using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to close a teller Account.
    /// </summary>
    public class CloseTellerAccountCommand : IRequest<ServiceResponse<AccountDto>>
    {
        public Account Account { get; set; }

        public CloseTellerAccountCommand(Account account)
        {
            Account = account;
        }
    }

}
