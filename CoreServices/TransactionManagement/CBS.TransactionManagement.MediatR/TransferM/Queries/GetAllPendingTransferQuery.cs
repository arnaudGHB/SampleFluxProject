using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllPendingTransferQuery : IRequest<ServiceResponse<List<TransferDto>>>
    {
    }
}
