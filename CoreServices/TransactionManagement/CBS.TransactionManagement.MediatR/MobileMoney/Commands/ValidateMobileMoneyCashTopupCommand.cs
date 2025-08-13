using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MobileMoney.Commands
{
    /// <summary>
    /// Represents a command to update a CashReplenishment.
    /// </summary>
    public class ValidateMobileMoneyCashTopupCommand : IRequest<ServiceResponse<MobileMoneyCashTopupDto>>
    {
        public string Id { get; set; }
        public string? RequestApprovalStatus { get; set; }
        public string? RequestApprovalNote { get; set; }
    }

}
