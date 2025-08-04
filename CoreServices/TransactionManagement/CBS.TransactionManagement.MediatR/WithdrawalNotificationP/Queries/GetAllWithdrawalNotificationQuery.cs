using CBS.TransactionManagement.Data.Dto.WithdrawalNotificationP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries.WithdrawalNotificationP
{
    public class GetAllWithdrawalNotificationQuery : IRequest<ServiceResponse<List<WithdrawalNotificationDto>>>
    {
    }
}
