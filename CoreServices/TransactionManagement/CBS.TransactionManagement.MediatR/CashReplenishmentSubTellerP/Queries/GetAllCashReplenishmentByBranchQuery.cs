using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Queries
{
    public class GetAllCashReplenishmentByBranchQuery : IRequest<ServiceResponse<List<SubTellerCashReplenishmentDto>>>
    {
        public string BranchId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

    }
}
