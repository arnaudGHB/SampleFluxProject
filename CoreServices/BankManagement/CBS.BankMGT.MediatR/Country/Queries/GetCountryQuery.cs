using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Country by its unique identifier.
    /// </summary>
    public class GetCountryQuery : IRequest<ServiceResponse<CountryDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Country to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
