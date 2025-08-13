using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific SubTellerProvioningHistory by its unique identifier.
    /// </summary>
    public class GetSubTellerProvioningHistoryQuery : IRequest<ServiceResponse<SubTellerProvioningHistoryDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the SubTellerProvioningHistory to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
