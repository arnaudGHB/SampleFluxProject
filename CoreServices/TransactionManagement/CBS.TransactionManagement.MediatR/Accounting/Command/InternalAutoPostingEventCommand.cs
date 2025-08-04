using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
  
    public class InternalAutoPostingEventCommand : IRequest<ServiceResponse<bool>>
    {
        public string EventCode { get; set; }
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; }
        public string Naration { get; set; }
        public InternalAutoPostingEventCommand(string eventCode, decimal amount, string transactionReference, string naration)
        {
            EventCode=eventCode;
            Amount=amount;
            TransactionReference=transactionReference;
            Naration=naration;
        }

    }
}
