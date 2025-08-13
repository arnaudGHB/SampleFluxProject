using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllTransactionsByDatesQuery : IRequest<ServiceResponse<List<TransactionDto>>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
