using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Teller by its unique identifier.
    /// </summary>
    public class GetTillStatusQuery : IRequest<ServiceResponse<List<TillOpenAndCloseOfDayDto>>>
    {
        public bool ByBranch { get; set; }
        public bool ByTeller { get; set; }
        public string QueryParameter { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
