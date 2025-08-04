using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to update a Transaction.
    /// </summary>
    public class UpdateAccountStatusCommand : IRequest<ServiceResponse<AccountDto>>
    {
        public UpdateAccountStatusCommand(string AccountNumber, string Status)
        {
            this.AccountNumber = AccountNumber;
            this.Status = Status;
        }

        public string AccountNumber { get; set; }
        public string Status { get; set; }
    }

}
