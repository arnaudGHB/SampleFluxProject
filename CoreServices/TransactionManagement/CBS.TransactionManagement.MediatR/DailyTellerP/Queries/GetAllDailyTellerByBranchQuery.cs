using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyTellerP.Queries
{
    public class GetAllDailyTellerByBranchQuery : IRequest<ServiceResponse<List<DailyTellerDto>>>
    {
        public string BranchId { get; set; }
    }
}
