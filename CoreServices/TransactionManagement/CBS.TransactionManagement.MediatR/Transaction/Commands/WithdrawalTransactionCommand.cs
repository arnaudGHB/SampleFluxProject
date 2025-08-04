using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class WithdrawalTransactionCommand : IRequest<ServiceResponse<TransactionDto>>
    {
        public CurrencyNotesRequest CurrencyNotes { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; } 
        public string WithDrawalType { get; set; }
        public string? Note { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
    }

}
