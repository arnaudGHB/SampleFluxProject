using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class TellerTransferTransactionCommand : IRequest<ServiceResponse<TransactionDto>>
    {
        public decimal Amount { get; set; }
        public Account primaryTellerAccount { get; set; }
        public Account subTellerAccount { get; set; }
        public string CurrencyNotesId { get; set; }
        //all args constructor
        public TellerTransferTransactionCommand(decimal amount, Account primaryTellerAccount, Account subTellerAccount, string currencyNotesId)
        {
            Amount = amount;
            this.primaryTellerAccount = primaryTellerAccount;
            this.subTellerAccount = subTellerAccount;
            CurrencyNotesId = currencyNotesId;
        }

    }

}
