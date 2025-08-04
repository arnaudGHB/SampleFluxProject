using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Currency by its unique identifier.
    /// </summary>
    public class GetCurrencyQuery : IRequest<ServiceResponse<CurrencyDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Currency to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
