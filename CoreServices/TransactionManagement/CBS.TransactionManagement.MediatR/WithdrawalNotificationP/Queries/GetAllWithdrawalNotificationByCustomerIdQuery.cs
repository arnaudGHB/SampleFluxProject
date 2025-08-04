using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries.WithdrawalNotificationP
{
    public class GetAllWithdrawalNotificationByCustomerIdQuery : IRequest<ServiceResponse<List<WithdrawalNotificationDto>>>
    {
        public string Id { get; set; }
    }
}
