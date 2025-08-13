using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.HolyDayP.Queries
{
    public class GetAllHolyDayQuery : IRequest<ServiceResponse<List<HolyDayDto>>>
    {
        public bool ByBranch { get; set; }
        public string? BranchId { get; set; }
    }
}
