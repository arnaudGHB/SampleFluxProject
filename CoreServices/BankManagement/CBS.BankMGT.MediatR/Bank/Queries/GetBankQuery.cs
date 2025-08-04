using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Queries
{
    /// <summary>
    /// Represents a query to retrieve a specific Bank by its unique identifier.
    /// </summary>
    public class GetBankQuery : IRequest<ServiceResponse<BankDto>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Bank to be retrieved.
        /// </summary>
        public string Id { get; set; }
    }
}
