using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Teller by its unique identifier.
    /// </summary>
    public class GetTellerOpenningAndClossingQuery : IRequest<ServiceResponse<List<OpenningAnclClossingTillDto>>>
    {
        public bool ByBracnch { get; set; }
        public string BranchId { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
