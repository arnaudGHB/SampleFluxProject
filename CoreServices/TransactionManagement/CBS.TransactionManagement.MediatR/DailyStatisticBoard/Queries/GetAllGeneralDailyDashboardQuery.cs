using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyStatisticBoard.Queries
{
    public class GetAllGeneralDailyDashboardQuery : IRequest<ServiceResponse<List<GeneralDailyDashboardDto>>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }
}
