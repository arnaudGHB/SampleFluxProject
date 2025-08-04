using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands.WithdrawalNotificationP
{
    /// <summary>
    /// Represents a command to add a new WithdrawalLimits.
    /// </summary>
    public class ValidationWithdrawalNotificationCommand : IRequest<ServiceResponse<WithdrawalNotificationDto>>
    {
        public string Id { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalComment { get; set; }
    }

}
