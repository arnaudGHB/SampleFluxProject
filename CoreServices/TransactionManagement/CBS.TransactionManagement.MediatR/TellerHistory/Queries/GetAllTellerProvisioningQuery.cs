using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllTellerProvisioningQuery : IRequest<ServiceResponse<List<PrimaryTellerProvisioningHistoryDto>>>
    {
        public string QueryString { get; set; }
        public string TellerId { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string BranchId { get; set; }
        public bool IsPrimaryTeller { get; set; }
        public bool IsByBranch { get; set; }
    }
}
