using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.otherCashIn.Queries
{
    public class GetAllOtherTransactionQuery : IRequest<ServiceResponse<List<CashDepositParameterDto>>>
    {
    }
}
