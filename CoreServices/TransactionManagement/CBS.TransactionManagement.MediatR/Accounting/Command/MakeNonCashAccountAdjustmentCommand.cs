using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
  
    public class MakeNonCashAccountAdjustmentCommand : IRequest<ServiceResponse<bool>>
    {
        public string ChartOfAccountId { get; set; }
        public string ProductId { get; set; }
        public string Source { get; set; }
        public string BookingDirection { get; set; }
        public decimal Amount { get; set; }
        public string Narration { get; set; }
        public string TransactionReference { get; set; }
        public string MemberReference { get; set; }
        public DateTime TransactionDate { get; set; }
        
    }
}
