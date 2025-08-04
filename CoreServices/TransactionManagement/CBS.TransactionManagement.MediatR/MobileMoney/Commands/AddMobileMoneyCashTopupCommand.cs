using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MobileMoney.Commands
{
    /// <summary>
    /// Represents a command to add a new CashReplenishment.
    /// </summary>
    public class AddMobileMoneyCashTopupCommand : IRequest<ServiceResponse<MobileMoneyCashTopupDto>>
    {
        public decimal Amount { get; set; }
        public string? OperatorType { get; set; }//MTN Or Orange
        public string? SourceType { get; set; }//GAV, OtherTransfer, HeadOffice, Wirering, M2Float
        public string BranchId { get; set; }
        public string? RequestNote { get; set; }
        public string? MobileMoneyTransactionId { get; set; }
        public string? AccountNumber { get; set; }
    }

}
