using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.HolyDayRecurringP.Queries
{
    public class GetAllHolyDayRecurringQuery : IRequest<ServiceResponse<List<HolyDayRecurringDto>>>
    {
        public bool ByBranch { get; set; }
        public string? BranchId { get; set; }
    }
}
