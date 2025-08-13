using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllTransactionsByDatesAndCustomerIDQuery : IRequest<ServiceResponse<List<TransactionDto>>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string CustomerID { get; set; }
    }
}
