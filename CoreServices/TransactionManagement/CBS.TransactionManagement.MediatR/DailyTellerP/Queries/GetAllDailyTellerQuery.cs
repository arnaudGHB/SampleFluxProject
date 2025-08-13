using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.DailyTellerP.Queries
{
    public class GetAllDailyTellerQuery : IRequest<ServiceResponse<List<DailyTellerDto>>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
