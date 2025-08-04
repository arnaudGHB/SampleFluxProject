using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific SubTellerProvioningHistory by its unique identifier.
    /// </summary>
    public class GetSubTellerProvioningHistoryByUserIDQuery : IRequest<ServiceResponse<List<SubTellerProvioningHistoryDto>>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the SubTellerProvioningHistory to be retrieved.
        /// </summary>
        public string UserInchargeId { get; set; }
    }
}
