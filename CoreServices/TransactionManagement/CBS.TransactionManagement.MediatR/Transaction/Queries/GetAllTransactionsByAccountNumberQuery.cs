using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllTransactionsByAccountNumberQuery : IRequest<ServiceResponse<List<TransactionDto>>>
    {
        public string AccountNumber { get; set; }
    }
}
