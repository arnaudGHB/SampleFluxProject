using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Region by its unique identifier.
    /// </summary>
    public class GetRegionQuery : IRequest<ServiceResponse<RegionDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Region to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
