using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Town by its unique identifier.
    /// </summary>
    public class GetTownQuery : IRequest<ServiceResponse<TownDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Town to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
