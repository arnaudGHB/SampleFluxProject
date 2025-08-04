using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
  
    public class CashInitializationCommand : IRequest<ServiceResponse<CashInitializationResponse>>
    {
        public decimal Amount { get; set; }
        public string TransactionReference { get; set; }
        public string Naration { get; set; }
        public decimal AmountInVault { get; set; }
        public bool CanProceed { get; set; }
        public DateTime AccountingDate { get; set; }
        public CashInitializationCommand(decimal amountInHand, string transactionReference, string naration, bool canProceed, DateTime accountingDate, decimal amountInVault)
        {
            Amount=amountInHand;
            TransactionReference=transactionReference;
            Naration=naration;
            CanProceed=canProceed;
            AccountingDate=accountingDate;
            AmountInVault=amountInVault;
        }
    }
    public class CashInitializationResponse
    {
        public string Naration { get; set; }
        public bool Status { get; set; }
    }
}

