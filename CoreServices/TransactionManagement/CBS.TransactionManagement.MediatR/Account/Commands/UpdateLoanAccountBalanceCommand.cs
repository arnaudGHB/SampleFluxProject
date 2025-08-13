using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to update a Transaction.
    /// </summary>
    public class UpdateAccountBalanceCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsDebit { get; set; }
        public string? LoastOperation { get; set; }

    }

}
