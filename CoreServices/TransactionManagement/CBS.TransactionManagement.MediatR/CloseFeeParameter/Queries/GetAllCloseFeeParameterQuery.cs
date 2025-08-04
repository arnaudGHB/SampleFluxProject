using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllCloseFeeParameterQuery : IRequest<ServiceResponse<List<CloseFeeParameterDto>>>
    {
    }
}
