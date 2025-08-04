using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.FeeP.Queries
{
    public class GetAllFeeQuery : IRequest<ServiceResponse<List<FeeDto>>>
    {
    }
}
