using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllTransactionsByDatesAndBranchQuery : IRequest<ServiceResponse<List<TransactionDto>>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string BranchID { get; set; }
        public string? TellerId { get; set; }
        public bool IsByDate { get; set; }
        public bool ByBranchId { get; set; }
        public bool ByTellerId { get; set; }
        public bool UseAccountingDate { get; set; }

    }
}
