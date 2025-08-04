using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Account.
    /// </summary>
    public class AddAccountCommand : IRequest<ServiceResponse<AccountDto>>
    {
        public string ProductId { get; set; }
        public string CustomerId { get; set; }
        public string BankId { get; set; }
        public string BranchId { get; set; }
        public string CustomerName { get; set; }
        public string? AccountId { get; set; }
        public bool IsRemoveAccount { get; set; }
    }

}
