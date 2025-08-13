using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries
{
    public class GetAllCashReplenishmentPrimaryTellerByBranchQuery : IRequest<ServiceResponse<List<CashReplenishmentPrimaryTellerDto>>>
    {
        public string BranchId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }
}
