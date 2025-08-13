using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllDepositLimitQuery : IRequest<ServiceResponse<List<CashDepositParameterDto>>>
    {
    }
}
