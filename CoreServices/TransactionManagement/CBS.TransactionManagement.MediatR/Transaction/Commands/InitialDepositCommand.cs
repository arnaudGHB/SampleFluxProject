using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class InitialDepositCommand : IRequest<ServiceResponse<TransactionDto>>
    {

        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public CurrencyNotesRequest CurrencyNotes { get; set; }
        public string BranchId { get; set; }
        public string BankId { get; set; }
        public string DepositType { get; set; }
    }


    
}
