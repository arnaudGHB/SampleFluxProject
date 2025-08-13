using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries.WithdrawalNotificationP
{
    /// <summary>
    /// Represents a query to retrieve a specific WithdrawalLimits by its unique identifier.
    /// </summary>
    public class GetWithdrawalNotificationQuery : IRequest<ServiceResponse<WithdrawalNotificationDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the WithdrawalLimits to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
