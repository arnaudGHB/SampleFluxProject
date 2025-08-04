using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific BankingZone by its unique identifier.
    /// </summary>
    public class GetBankingZoneQuery : IRequest<ServiceResponse<BankingZoneDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the BankingZone to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
