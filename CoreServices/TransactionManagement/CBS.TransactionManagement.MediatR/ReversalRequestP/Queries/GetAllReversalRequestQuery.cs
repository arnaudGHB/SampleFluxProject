using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries.ReversalRequestP
{
    public class GetAllReversalRequestQuery : IRequest<ServiceResponse<List<ReversalRequestDto>>>
    {
        public string QueryString { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public bool IsBranch { get; set; }
        public bool IsByDate { get; set; }
        public string BranchId { get; set; }
    }
}
