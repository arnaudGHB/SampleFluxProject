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
    public class CashDeskWithdrawalNotificationCommand : IRequest<ServiceResponse<TransactionDto>>
    {
        public string Id { get; set; }
    }

}
