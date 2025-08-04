using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Organization by its unique identifier.
    /// </summary>
    public class GetOrganizationQuery : IRequest<ServiceResponse<OrganizationDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Organization to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
