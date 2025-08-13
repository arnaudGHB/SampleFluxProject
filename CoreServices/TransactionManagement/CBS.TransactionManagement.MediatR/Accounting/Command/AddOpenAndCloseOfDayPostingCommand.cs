using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
namespace CBS.TransactionManagement.Command
{
    public class AddOpenAndCloseOfDayPostingCommand:IRequest<ServiceResponse<bool>>
    {
        public string TransactionReferenceId { get; set; }
        public string EventCode { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountDifference { get; set; }
        public AddOpenAndCloseOfDayPostingCommand(string transactionReferenceId = null, string eventCode = "OOD", decimal amount = 0, decimal amountDifference = 0)
        {
            TransactionReferenceId = transactionReferenceId;
            EventCode = eventCode;
            Amount = amount;
            AmountDifference = amountDifference;
        }
    }
 
}
