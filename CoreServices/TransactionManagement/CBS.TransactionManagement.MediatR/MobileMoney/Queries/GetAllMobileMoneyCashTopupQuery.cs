using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CBS.TransactionManagement.MediatR.MobileMoney.Queries
{
    public class GetAllMobileMoneyCashTopupQuery : IRequest<ServiceResponse<List<MobileMoneyCashTopupDto>>>
    {
        public string? QueryParameter { get; set; }
        public bool ByBranch { get; set; }
        public string? BranchId { get; set; }
    }
}
