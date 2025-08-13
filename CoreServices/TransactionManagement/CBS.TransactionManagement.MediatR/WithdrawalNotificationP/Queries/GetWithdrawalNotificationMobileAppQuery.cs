using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries.WithdrawalNotificationP
{
    /// <summary>
    /// Represents a query to retrieve a specific WithdrawalLimits by its unique identifier.
    /// </summary>
    public class GetWithdrawalNotificationMobileAppQuery : IRequest<ServiceResponse<WithdrawalNotificationAndriodDto>>
    {
        public string AccountNUmber { get; set; }
        public string CustomerReference { get; set; }
    }
}
