using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyStatisticBoard.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific TempAccount by its unique identifier.
    /// </summary>
    public class GeneralDailyDashboardByBranchQuery : IRequest<ServiceResponse<GeneralDailyDashboardDto>>
    {
       
        public string BranchId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }
}
