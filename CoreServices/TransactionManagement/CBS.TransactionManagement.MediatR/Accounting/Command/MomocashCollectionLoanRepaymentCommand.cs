using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
    public class PaymentCollection
    {
        public string EventCode { get; set; }
        public decimal Amount { get; set; }
        public string ProductId { get; set; }
        public bool IsComission { get; set; }
        public string Narration { get; set; }
    }
    public class MobileMoneyCollectionOperationCommand : IRequest<ServiceResponse<bool>>
    {
        public string TellerSources { get; set; }
        public string TransactionReferenceId { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<PaymentCollection>? PaymentCollection { get; set; }
    }

}







