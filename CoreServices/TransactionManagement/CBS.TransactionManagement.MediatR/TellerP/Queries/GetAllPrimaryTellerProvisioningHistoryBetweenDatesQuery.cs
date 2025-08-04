using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Queries
{
    public class GetAllPrimaryTellerProvisioningHistoryBetweenDatesQuery : IRequest<ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
