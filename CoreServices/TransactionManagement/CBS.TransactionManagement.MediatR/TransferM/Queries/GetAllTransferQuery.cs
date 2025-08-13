using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.TransferM.Queries
{
    public class GetAllTransferQuery : IRequest<ServiceResponse<List<TransferDto>>>
    {
    }
}
