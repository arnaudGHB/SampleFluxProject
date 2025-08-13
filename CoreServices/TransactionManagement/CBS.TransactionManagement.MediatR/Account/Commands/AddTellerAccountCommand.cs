using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Teller Account.
    /// </summary>
    public class AddTellerAccountCommand : IRequest<ServiceResponse<AccountDto>>
    {
        public string ProductId { get; set; }
        public string TellerId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
    }

}
