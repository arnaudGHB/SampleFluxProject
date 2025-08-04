using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAllTellerOperationsQuery : IRequest<ServiceResponse<List<TellerOperationGL>>>
    {
        public string QueryString { get; set; }
        public string TellerId { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string BranchId { get; set; }
        public bool IsByTeller { get; set; }
        public bool IsByBranch { get; set; }
        public bool IsByDate { get; set; }
    }
}
