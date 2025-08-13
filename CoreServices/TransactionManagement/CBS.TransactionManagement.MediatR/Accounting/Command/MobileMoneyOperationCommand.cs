using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Command
{
   
    public class MobileMoneyOperationCommand : IRequest<ServiceResponse<bool>>
    {
        public string TransactionReference { get; set; }
        public decimal Amount { get; set; }
        public string OperationType { get; set; }
        public string TellerSources { get; set; }
        public DateTime TransactionDate { get; set; }
    }
   
}







