using CBS.AccountManagement.MediatR.AccountingEntry.Commands;
using MediatR;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.MediatR.Commands
{
    public class PaymentCollection
    {
        public string ProductId { get; set; }
        public string EventCode { get; set; }
        public bool IsComission { get; set; }
        public decimal Amount { get; set; }
        public string? Naration { get; set; }

        public string GetOperationEventCode()
        {
            return   this.EventCode;
        }
        public string GetOperationEventCode(string productId)
        {
            return productId + "@" + this.EventCode;
        }
    }

    public class MobileMoneyCollectionOperationCommand : IRequest<ServiceResponse<bool>>
    
    {
        public string TellerSources { get; set; }
        public string MemberReference { get; set; }
        public string TransactionReferenceId { get; set; }
        public List<PaymentCollection>? PaymentCollection { get; set; }
        public DateTime TransactionDate { get; set; }

    }

    public enum OperationType
    {
        LoanRepayement,
        CashIn
    }
}