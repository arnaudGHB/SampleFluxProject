using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllTransactionsByAccountQuery : IRequest<ServiceResponse<List<TransactionDto>>>
    {
        public string AccountId { get; set; }
    }
}
