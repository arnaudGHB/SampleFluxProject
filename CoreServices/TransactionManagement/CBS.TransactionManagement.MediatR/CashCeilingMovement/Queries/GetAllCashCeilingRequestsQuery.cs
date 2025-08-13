using CBS.TransactionManagement.Data.Dto.CashCeilingMovement;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CashCeilingMovement.Queries
{
    public class GetAllCashCeilingRequestsQuery : IRequest<ServiceResponse<List<CashCeilingRequestDto>>>
    {
        public string BranchId { get; set; } // Optional parameter to filter by branch
        public string Status { get; set; } // Optional parameter to filter by status
        public string UserId { get; set; }
        public string RequestType { get; set; }
    }
}
