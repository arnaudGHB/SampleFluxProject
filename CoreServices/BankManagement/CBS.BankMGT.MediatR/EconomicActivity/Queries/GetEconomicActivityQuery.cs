using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific EconomicActivity by its unique identifier.
    /// </summary>
    public class GetEconomicActivityQuery : IRequest<ServiceResponse<EconomicActivityDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the EconomicActivity to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
