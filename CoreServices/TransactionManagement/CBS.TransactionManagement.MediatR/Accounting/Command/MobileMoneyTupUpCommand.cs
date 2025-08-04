using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
   
    public class MobileMoneyTupUpCommand : IRequest<ServiceResponse<bool>>
    {
        public string ChartOfAccountIdFrom { get; set; }
        public string TransactionReferenceId { get; set; }
        public string ChartOfAccountIdTo { get; set; }
        public string Direction { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
   
}







