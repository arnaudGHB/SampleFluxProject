using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Subdivision by its unique identifier.
    /// </summary>
    public class GetSubdivisionQuery : IRequest<ServiceResponse<SubdivisionDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Subdivision to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
